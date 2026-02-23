using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.DataAccess;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<OrderParty> OrderParties => Set<OrderParty>();
    public DbSet<PersonParty> PersonParties => Set<PersonParty>();
    public DbSet<CompanyParty> CompanyParties => Set<CompanyParty>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<AirTicket> AirTickets => Set<AirTicket>();
    public DbSet<HotelBooking> HotelBookings => Set<HotelBooking>();
    public DbSet<ExtraService> ExtraServices => Set<ExtraService>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PriceCurrency> PriceCurrencies => Set<PriceCurrency>();
    public DbSet<CustomerBankRequisites> CustomerBankRequisites { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---- Order Party inheritance ----
        modelBuilder.Entity<OrderParty>()
            .HasDiscriminator<string>("PartyType")
            .HasValue<PersonParty>("Person")
            .HasValue<CompanyParty>("Company");

        modelBuilder.Entity<OrderParty>()
            .Property<string>("PartyType")
            .HasMaxLength(20);

        // ---- Tour configuration ----
        modelBuilder.Entity<Tour>(b =>
        {
            b.Property(t => t.StartDate).HasColumnType("date");
            b.Property(t => t.EndDate).HasColumnType("date");

            b.HasMany(t => t.AirTickets)
                .WithOne(d => d.Tour)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(t => t.HotelBookings)
                .WithOne(d => d.Tour)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(t => t.ExtraServices)
                .WithOne(d => d.Tour)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            // NEW: Tour ↔ Passengers
            b.HasMany(t => t.Passengers)
                .WithOne(p => p.Tour!)
                .HasForeignKey(p => p.TourId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Order>()
           .HasOne(o => o.CustomerBankRequisites)
           .WithMany()
           .HasForeignKey(o => o.CustomerBankRequisitesId)
           .OnDelete(DeleteBehavior.SetNull);

        // ---- Supplier–Tour relationship ----
        modelBuilder.Entity<Tour>()
            .HasOne(t => t.TourSupplier)
            .WithMany(s => s.Tours)
            .HasForeignKey(t => t.TourSupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- Air Ticket ----
        modelBuilder.Entity<AirTicket>(b =>
        {
            b.Property(a => a.FlightDate).HasColumnType("date");
        });

        // ---- Hotel Booking ----
        modelBuilder.Entity<HotelBooking>(b =>
        {
            b.Property(a => a.CheckIn).HasColumnType("date");
            b.Property(a => a.CheckOut).HasColumnType("date");
        });

        // ---- Order ----
        modelBuilder.Entity<Order>(b =>
        {
            b.HasIndex(o => o.OrderNumber).IsUnique();

            // PostgreSQL-native optimistic concurrency using xmin
            b.Property<uint>("xmin")
                .IsRowVersion()
                .HasColumnName("xmin");

            b.HasOne(o => o.OrderParty)
                .WithMany()
                .HasForeignKey(o => o.OrderPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(o => o.Tour)
                .WithMany()
                .HasForeignKey(o => o.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            // SellPriceInGel is a simple decimal now; no value object
            b.Property(o => o.SellPriceInGel)
                .HasColumnType("numeric(18,2)");

            // Order ↔ Payments
            b.HasMany(o => o.Payments)
                .WithOne(p => p.Order!)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---- Passenger ----
        modelBuilder.Entity<Passenger>(b =>
        {
            b.Property(p => p.BirthDate).HasColumnType("date");
        });

        // ---- Payment ----
        modelBuilder.Entity<Payment>(b =>
        {
            b.Property(p => p.PaidDate).HasColumnType("date");

            b.HasOne(p => p.PriceCurrency)
                .WithMany()
                .HasForeignKey(p => p.PriceCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // (No ExchangeRate entity anymore)
        // If you want, you can add mapping for PriceCurrency dates etc., but not required.
    }
}
