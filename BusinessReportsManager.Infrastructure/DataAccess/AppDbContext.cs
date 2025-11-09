using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Infrastructure.DataAccess;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<OrderParty> OrderParties => Set<OrderParty>();
    public DbSet<PersonParty> PersonParties => Set<PersonParty>();
    public DbSet<CompanyParty> CompanyParties => Set<CompanyParty>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<AirTicket> AirTickets => Set<AirTicket>();
    public DbSet<HotelBooking> HotelBookings => Set<HotelBooking>();
    public DbSet<ExtraService> ExtraServices => Set<ExtraService>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

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

            b.HasMany(t => t.Destinations)
                .WithOne(d => d.Tour)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.Cascade);

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
        });

        // ---- Supplier–Tour relationship ----
        modelBuilder.Entity<Tour>()
            .HasOne(t => t.TourSupplier)
            .WithMany(s => s.Tours)
            .HasForeignKey(t => t.TourSupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- Destination ----
        modelBuilder.Entity<Destination>(b =>
        {
            b.Property(d => d.Country).HasMaxLength(100);
            b.Property(d => d.City).HasMaxLength(100);
        });

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

            // ✅ PostgreSQL-native optimistic concurrency using xmin
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

            // ---- Money / value objects ----
            b.OwnsOne(o => o.SellPrice, m =>
            {
                m.Property(x => x.Amount).HasColumnType("numeric(18,2)");
                m.Property(x => x.Currency)
                    .HasConversion<string>()
                    .HasMaxLength(3);
            });

            b.OwnsOne(o => o.TicketSelfCost, m =>
            {
                m.Property(x => x.Amount).HasColumnType("numeric(18,2)");
                m.Property(x => x.Currency)
                    .HasConversion<string>()
                    .HasMaxLength(3);
            });

            b.HasMany(o => o.Passengers)
                .WithOne(p => p.Order!)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

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
            b.OwnsOne(p => p.Amount, m =>
            {
                m.Property(x => x.Amount).HasColumnType("numeric(18,2)");
                m.Property(x => x.Currency)
                    .HasConversion<string>()
                    .HasMaxLength(3);
            });

            b.Property(p => p.PaidDate).HasColumnType("date");
        });

        // ---- Exchange Rate ----
        modelBuilder.Entity<ExchangeRate>(b =>
        {
            b.Property(x => x.FromCurrency)
                .HasConversion<string>()
                .HasMaxLength(3);

            b.Property(x => x.ToCurrency)
                .HasConversion<string>()
                .HasMaxLength(3);

            b.Property(x => x.EffectiveDate).HasColumnType("date");

            b.HasIndex(x => new { x.FromCurrency, x.ToCurrency, x.EffectiveDate });
        });
    }
}
