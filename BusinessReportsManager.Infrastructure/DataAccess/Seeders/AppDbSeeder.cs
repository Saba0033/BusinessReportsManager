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
        // ==========================================================
        // 1. ROLES
        // ==========================================================
        string[] roles = { "Employee", "Accountant", "Supervisor" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ==========================================================
        // 2. USERS
        // ==========================================================
        async Task EnsureUser(string email, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser { Email = email, UserName = email };
                await userManager.CreateAsync(user, "P@ssword1");
            }

            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }

        await EnsureUser("employee@demo.local", "Employee");
        await EnsureUser("accountant@demo.local", "Accountant");
        await EnsureUser("supervisor@demo.local", "Supervisor");

        // ==========================================================
        // Prevent Duplicate SEED Runs
        // ==========================================================
        if (await db.Orders.AnyAsync())
            return;

        // ==========================================================
        // 3. SUPPLIER
        // ==========================================================
        var supplier = new Supplier
        {
            Name = "GeoTravel Supplier",
            ContactEmail = "info@geotravel.local",
            Phone = "+995555123456"
        };
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();

        // ==========================================================
        // 4. ORDER PARTY (Person)
        // ==========================================================
        var party = new PersonParty
        {
            Email = "customer@demo.local",
            Phone = "+995555987654",
            FirstName = "Nino",
            LastName = "Beridze",
            BirthDate = new DateOnly(1990, 1, 1)
        };
        db.PersonParties.Add(party);
        await db.SaveChangesAsync();

        // ==========================================================
        // 5. TOUR (with all children)
        // ==========================================================
        var tour = new Tour
        {
            Name = "Georgia Golden Tour (Tbilisi + Batumi + Kutaisi)",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(17)),
            PassengerCount = 2,
            TourSupplier = supplier
        };
        db.Tours.Add(tour);
        await db.SaveChangesAsync();

        // -------- Passengers --------
        db.Passengers.AddRange(
            new Passenger
            {
                TourId = tour.Id,
                FirstName = "Nino",
                LastName = "Beridze",
                BirthDate = new DateOnly(1990, 1, 1),
                DocumentNumber = "AB1234567"
            },
            new Passenger
            {
                TourId = tour.Id,
                FirstName = "Giorgi",
                LastName = "Beridze",
                BirthDate = new DateOnly(2010, 5, 10),
                DocumentNumber = "CD8910111"
            }
        );

        // -------- Air Tickets --------
        var ticketPrice1 = new PriceCurrency
        {
            Currency = Currency.USD,
            Amount = 250,
            ExchangeRateToGel = 2.7m,
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        db.PriceCurrencies.Add(ticketPrice1);

        var ticket1 = new AirTicket
        {
            TourId = tour.Id,
            CountryFrom = "Georgia",
            CountryTo = "Turkey",
            CityFrom = "Tbilisi",
            CityTo = "Istanbul",
            FlightDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            FlightCompanyName = "Turkish Airlines",
            Quantity = 2,
            PriceCurrency = ticketPrice1
        };
        db.AirTickets.Add(ticket1);

        var ticketPrice2 = new PriceCurrency
        {
            Currency = Currency.USD,
            Amount = 300,
            ExchangeRateToGel = 2.7m,
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        db.PriceCurrencies.Add(ticketPrice2);

        var ticket2 = new AirTicket
        {
            TourId = tour.Id,
            CountryFrom = "Turkey",
            CountryTo = "Georgia",
            CityFrom = "Istanbul",
            CityTo = "Batumi",
            FlightDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(17)),
            FlightCompanyName = "Turkish Airlines",
            Quantity = 2,
            PriceCurrency = ticketPrice2
        };
        db.AirTickets.Add(ticket2);

        // -------- Hotel Booking --------
        var hotelPrice = new PriceCurrency
        {
            Currency = Currency.USD,
            Amount = 450,
            ExchangeRateToGel = 2.7m,
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        db.PriceCurrencies.Add(hotelPrice);

        var hotel = new HotelBooking
        {
            TourId = tour.Id,
            HotelName = "Radisson Blu Batumi",
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(11)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)),
            PriceCurrency = hotelPrice
        };
        db.HotelBookings.Add(hotel);

        // -------- Extra Service --------
        var extraPrice = new PriceCurrency
        {
            Currency = Currency.GEL,
            Amount = 120,
            ExchangeRateToGel = 1m,
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        db.PriceCurrencies.Add(extraPrice);

        var extra = new ExtraService
        {
            TourId = tour.Id,
            Description = "Airport pickup",
            PriceCurrency = extraPrice
        };
        db.ExtraServices.Add(extra);

        await db.SaveChangesAsync();

        // ==========================================================
        // 6. ORDER
        // ==========================================================
        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-0001",
            OrderParty = party,
            Tour = tour,
            Source = "SeedData",
            SellPriceInGel = 3000m,
            Status = OrderStatus.Open
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        // ==========================================================
        // 7. PAYMENTS (NEW MODEL)
        // ==========================================================

        // Payment 1
        var payPrice1 = new PriceCurrency
        {
            Currency = Currency.GEL,
            Amount = 500,
            ExchangeRateToGel = 1m,
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        db.PriceCurrencies.Add(payPrice1);

        db.Payments.Add(new Payment
        {
            OrderId = order.Id,
            PriceCurrency = payPrice1,
            BankName = "TBC",
            PaidDate = DateOnly.FromDateTime(DateTime.UtcNow)
        });

        // Payment 2
        var payPrice2 = new PriceCurrency
        {
            Currency = Currency.USD,
            Amount = 300,
            ExchangeRateToGel = 2.7m,
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        db.PriceCurrencies.Add(payPrice2);

        db.Payments.Add(new Payment
        {
            OrderId = order.Id,
            PriceCurrency = payPrice2,
            BankName = "BOG",
            PaidDate = DateOnly.FromDateTime(DateTime.UtcNow)
        });

        await db.SaveChangesAsync();
    }
}
