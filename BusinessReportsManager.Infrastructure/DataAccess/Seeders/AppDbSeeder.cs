using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Domain.ValueObjects;
using BusinessReportsManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using BusinessReportsManager.Infrastructure.DataAccess;

namespace BusinessReportsManager.Infrastructure.DataAccess.Seeders;

public static class AppDbSeeder
{
    public static async Task SeedAsync(AppDbContext db, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roles = new[] { "Employee", "Accountant", "Supervisor" };
        foreach (var role in roles)
        {
            if (!await roleManager.Roles.AnyAsync(r => r.Name == role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        async Task<AppUser> EnsureUserAsync(string email, string role)
        {
            var u = await userManager.FindByEmailAsync(email);
            if (u == null)
            {
                u = new AppUser { UserName = email, Email = email };
                await userManager.CreateAsync(u, "P@ssw0rd!");
            }
            if (!await userManager.IsInRoleAsync(u, role))
                await userManager.AddToRoleAsync(u, role);
            return u;
        }

        var employee = await EnsureUserAsync("employee1@demo.local", "Employee");
        var accountant = await EnsureUserAsync("accountant1@demo.local", "Accountant");
        var supervisor = await EnsureUserAsync("supervisor1@demo.local", "Supervisor");

        if (!await db.Banks.AnyAsync())
        {
            db.Banks.AddRange(
                new Bank { Name = "TBC Bank", Swift = "TBCBGE22", AccountNumber = "GE00TB0000000000000000" },
                new Bank { Name = "Bank of Georgia", Swift = "BGEEGE22", AccountNumber = "GE00BG0000000000000000" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Suppliers.AnyAsync())
        {
            db.Suppliers.AddRange(
                new Supplier { Name = "GeoTravel Supplier", ContactEmail = "contact@geotravel.local" },
                new Supplier { Name = "Caucasus Tours", ContactEmail = "sales@caucasustours.local" }
            );
            await db.SaveChangesAsync();
        }

        var party = await db.PersonParties.FirstOrDefaultAsync();
        if (party == null)
        {
            party = new PersonParty { Email = "customer@demo.local", FirstName = "Nino", LastName = "Beridze" };
            db.PersonParties.Add(party);
            await db.SaveChangesAsync();
        }

        var tour = await db.Tours.Include(t => t.Destinations).FirstOrDefaultAsync();
        if (tour == null)
        {
            tour = new Tour
            {
                Name = "Tbilisi & Batumi Highlights",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(10)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(17)),
                PassengerCount = 2
            };
            tour.Destinations.Add(new Destination { Country = "Georgia", City = "Tbilisi" });
            tour.Destinations.Add(new Destination { Country = "Georgia", City = "Batumi" });
            db.Tours.Add(tour);
            await db.SaveChangesAsync();
        }

        if (!await db.Orders.AnyAsync())
        {
            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow.Year}-0001",
                OrderPartyId = party.Id,
                TourId = tour.Id,
                Source = "Seed",
                SellPrice = new Money(1000m, Currency.GEL),
                TicketSelfCost = new Money(600m, Currency.GEL),
                OwnedByUserId = employee.Id,
                Status = OrderStatus.Open
            };

            order.Passengers.Add(new Passenger { FirstName = "Nino", LastName = "Beridze", IsPrimary = true, BirthDate = new DateOnly(1990,1,1) });
            order.Passengers.Add(new Passenger { FirstName = "Giorgi", LastName = "Beridze", IsPrimary = false });

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var tbc = await db.Banks.FirstAsync(b => b.Name.Contains("TBC"));
            db.Payments.Add(new Payment
            {
                OrderId = order.Id,
                Amount = new Money(300m, Currency.GEL),
                BankId = tbc.Id,
                PaidDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                Reference = "Advance"
            });
            await db.SaveChangesAsync();
        }

        if (!await db.ExchangeRates.AnyAsync())
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            db.ExchangeRates.AddRange(
                new ExchangeRate { FromCurrency = Currency.GEL, ToCurrency = Currency.USD, Rate = 0.37m, EffectiveDate = today.AddDays(-10) },
                new ExchangeRate { FromCurrency = Currency.USD, ToCurrency = Currency.GEL, Rate = 2.7m, EffectiveDate = today.AddDays(-10) },
                new ExchangeRate { FromCurrency = Currency.GEL, ToCurrency = Currency.EUR, Rate = 0.34m, EffectiveDate = today.AddDays(-10) },
                new ExchangeRate { FromCurrency = Currency.EUR, ToCurrency = Currency.GEL, Rate = 2.94m, EffectiveDate = today.AddDays(-10) }
            );
            await db.SaveChangesAsync();
        }
    }
}