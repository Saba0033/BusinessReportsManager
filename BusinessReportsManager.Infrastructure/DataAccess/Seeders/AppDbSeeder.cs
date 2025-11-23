using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.DataAccess.Seeders;

public static class AppDbSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // ---------------------------------------------
        // 1. Create Roles
        // ---------------------------------------------
        string[] roles = { "Employee", "Accountant", "Supervisor" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ---------------------------------------------
        // 2. Create Users (with real emails + safe password)
        // ---------------------------------------------
        async Task<AppUser> EnsureUser(string email, string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email
                };

                // Use guaranteed-valid password
                var create = await userManager.CreateAsync(user, "P@ssword1");

                if (!create.Succeeded)
                {
                    throw new Exception(
                        "User creation failed: " +
                        string.Join(", ", create.Errors.Select(e => e.Description))
                    );
                }
            }

            // Add to role
            if (!await userManager.IsInRoleAsync(user, role))
            {
                var addRole = await userManager.AddToRoleAsync(user, role);

                if (!addRole.Succeeded)
                {
                    throw new Exception(
                        "AddToRole failed: " +
                        string.Join(", ", addRole.Errors.Select(e => e.Description))
                    );
                }
            }

            return user;
        }

        var employee = await EnsureUser("employee@demo.local", "Employee");
        var accountant = await EnsureUser("accountant@demo.local", "Accountant");
        var supervisor = await EnsureUser("supervisor@demo.local", "Supervisor");

        // ---------------------------------------------
        // 3. Suppliers
        // ---------------------------------------------
        if (!await db.Suppliers.AnyAsync())
        {
            db.Suppliers.AddRange(
                new Supplier { Name = "GeoTravel Supplier", ContactEmail = "contact@geotravel.local" },
                new Supplier { Name = "Caucasus Tours", ContactEmail = "sales@caucasustours.local" }
            );

            await db.SaveChangesAsync();
        }

        // ---------------------------------------------
        // 4. Order Party (Person)
        // ---------------------------------------------
        var party = await db.PersonParties.FirstOrDefaultAsync();
        if (party == null)
        {
            party = new PersonParty
            {
                Email = "customer@demo.local",
                FirstName = "Nino",
                LastName = "Beridze",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            db.PersonParties.Add(party);
            await db.SaveChangesAsync();
        }

        // ---------------------------------------------
        // 5. Create Tour + Passengers
        // ---------------------------------------------
        var tour = await db.Tours.Include(t => t.Passengers).FirstOrDefaultAsync();
        if (tour == null)
        {
            var supplier = await db.Suppliers.FirstAsync();

            tour = new Tour
            {
                Name = "Tbilisi & Batumi Highlights",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(17)),
                PassengerCount = 2,
                TourSupplierId = supplier.Id
            };

            db.Tours.Add(tour);
            await db.SaveChangesAsync(); // IMPORTANT: Must save before passengers

            db.Passengers.AddRange(
                new Passenger
                {
                    TourId = tour.Id,
                    FirstName = "Nino",
                    LastName = "Beridze",
                    BirthDate = new DateOnly(1990, 1, 1)
                },
                new Passenger
                {
                    TourId = tour.Id,
                    FirstName = "Giorgi",
                    LastName = "Beridze"
                }
            );

            await db.SaveChangesAsync();
        }

        // ---------------------------------------------
        // 6. Order + Payment
        // ---------------------------------------------
        if (!await db.Orders.AnyAsync())
        {
            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow.Year}-0001",
                OrderPartyId = party.Id,
                TourId = tour.Id,
                Source = "Seed",
                SellPriceInGel = 1000m,
                Status = OrderStatus.Open
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            // Currency
            var curr = new PriceCurrency
            {
                Currency = Currency.GEL,
                Amount = 300m,
                ExchangeRateToGel = 1m,
                EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            db.PriceCurrencies.Add(curr);
            await db.SaveChangesAsync();

            // Payment
            db.Payments.Add(new Payment
            {
                OrderId = order.Id,
                PriceCurrencyId = curr.Id,
                BankName = "TBC",
                PaidDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Reference = "Advance"
            });

            await db.SaveChangesAsync();
        }
    }
}
