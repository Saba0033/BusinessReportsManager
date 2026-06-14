using System.Security.Claims;
using AutoMapper;
using BusinessReportsManager.Application.AbstractServices;
using BusinessReportsManager.Application.DTOs.Order;
using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Supplier;
using BusinessReportsManager.Application.Mappings;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.Enums;
using BusinessReportsManager.Infrastructure.DataAccess;
using BusinessReportsManager.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Tests;

/// <summary>Manual-rate FX stub for tests: GEL=1, otherwise honors the provided rate (default 1).</summary>
internal sealed class FakeExchangeRateService : IExchangeRateService
{
    public Task<decimal> GetRateToGelAsync(Currency currency, DateOnly? date = null, CancellationToken ct = default)
        => Task.FromResult(currency == Currency.GEL ? 1m : 1m);

    public Task<decimal> ResolveRateToGelAsync(Currency currency, decimal? manualRate, DateOnly? date = null, CancellationToken ct = default)
        => Task.FromResult(currency == Currency.GEL ? 1m : (manualRate is > 0 ? manualRate.Value : 1m));
}

public class OrderPartyNameTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly OrderService _service;
    private readonly Guid _userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public OrderPartyNameTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<AppProfile>()).CreateMapper();
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
            new Claim(ClaimTypes.Email, "supervisor@test.local"),
            new Claim("username", "supervisor")
        }, "Test"));

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var uow = new UnitOfWork(_db);
        _service = new OrderService(uow, mapper, accessor, new FakeExchangeRateService());
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateOrder_WithoutPersonalNumber_ShowsFullNameInReport()
    {
        var order = await _service.CreateFullOrderAsync(BuildOrderDto(
            fullName: "GIORGI BERIDZE",
            email: "giorgi.beridze@test.local",
            personalNumber: null));

        var report = await _service.GetByIdReportAsync(order.Id);

        Assert.NotNull(report);
        Assert.Equal("GIORGI BERIDZE", report!.ClientName);
    }

    [Fact]
    public async Task CreateOrder_MatchingEmailWithoutPersonalNumber_UpdatesClientNameInReport()
    {
        var existingParty = new PersonParty
        {
            Email = "repeat.customer@test.local",
            FirstName = string.Empty,
            LastName = string.Empty
        };
        _db.PersonParties.Add(existingParty);
        await _db.SaveChangesAsync();

        var order = await _service.CreateFullOrderAsync(BuildOrderDto(
            fullName: "NINO KAPANADZE",
            email: "repeat.customer@test.local",
            personalNumber: null));

        var report = await _service.GetByIdReportAsync(order.Id);
        var party = await _db.PersonParties.FindAsync(existingParty.Id);

        Assert.NotNull(report);
        Assert.NotNull(party);
        Assert.Equal("NINO KAPANADZE", report!.ClientName);
        Assert.Equal("NINO", party!.FirstName);
        Assert.Equal("KAPANADZE", party.LastName);
    }

    [Fact]
    public async Task CreateOrder_WithPersonalNumber_AppendsIdNextToNameInReport()
    {
        var order = await _service.CreateFullOrderAsync(BuildOrderDto(
            fullName: "Nino Beridze",
            email: "nino.withid@test.local",
            personalNumber: "61001012345"));

        var report = await _service.GetByIdReportAsync(order.Id);

        Assert.NotNull(report);
        Assert.Equal("Nino Beridze (61001012345)", report!.ClientName);
    }

    [Fact]
    public async Task CreateOrder_WithMultiCurrencyNets_ComputesProfitFromNetSumInGel()
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        var dto = new OrderCreateDto
        {
            Party = new PartyCreateDto { FullName = "FX CLIENT", Email = "fx@test.local" },
            Destination = "Greece",
            StartDate = start,
            EndDate = start.AddDays(7),
            PassengerCount = 1,
            Supplier = new SupplierCreateDto { Name = "Supplier" },
            Source = "UnitTest",
            SellPriceInGel = 5000m,
            Passengers = new(),
            TicketNet = 400m, TicketNetCurrency = Currency.USD, TicketNetRate = 2.75m,
            HotelNet = 600m, HotelNetCurrency = Currency.EUR, HotelNetRate = 3.0m,
            TransferNet = 150m, TransferNetCurrency = Currency.GEL,
            CruiseNet = 800m, CruiseNetCurrency = Currency.USD, CruiseNetRate = 2.75m,
            InsuranceNet = 100m,
            OtherServiceNet = 50m, OtherServiceNetCurrency = Currency.GEL
        };

        var order = await _service.CreateFullOrderAsync(dto);
        var report = await _service.GetByIdReportAsync(order.Id);

        // 1100 + 1800 + 150 + 2200 + 100 + 50 = 5400
        Assert.NotNull(report);
        Assert.Equal(1100m, report!.TicketNet);
        Assert.Equal(1800m, report.HotelNet);
        Assert.Equal(2200m, report.CruiseNet);
        Assert.Equal(5400m, report.TotalExpenses);
        Assert.Equal(-400m, report.Profit);

        // OrderDto exposes both original and GEL-converted values
        Assert.Equal(400m, order.TicketNet);
        Assert.Equal(Currency.USD, order.TicketNetCurrency);
        Assert.Equal(1100m, order.TicketNetInGel);
        Assert.Equal(5400m, order.TotalExpenseInGel);
        Assert.Equal(-400m, order.Profit);
    }

    [Fact]
    public async Task GenerateInvoiceExcel_ProducesNonEmptyFile()
    {
        var created = await _service.CreateFullOrderAsync(
            BuildOrderDto("INVOICE CLIENT", "invoice@test.local", "01010101010"));
        var order = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(order);

        var excel = new OrderExcelService();
        var bytes = excel.GenerateInvoiceExcel(order!);

        Assert.True(bytes.Length > 1000, "Invoice file should not be empty.");
    }

    [Fact]
    public async Task SearchCustomers_FindsBySurnameFragment_WithAutoFillDetails()
    {
        await _service.CreateFullOrderAsync(BuildOrderDto("Giorgi Beridze", "giorgi.b@test.local", "11111111111"));
        await _service.CreateFullOrderAsync(BuildOrderDto("Nino Kapanadze", "nino.k@test.local", null));

        var results = await _service.SearchCustomersAsync("ber", 10);

        var match = Assert.Single(results, c => c.FullName == "Giorgi Beridze");
        Assert.Equal("giorgi.b@test.local", match.Email);
        Assert.Equal("11111111111", match.PersonalNumber);
        Assert.DoesNotContain(results, c => c.FullName == "Nino Kapanadze");
    }

    [Fact]
    public async Task SearchSuppliers_ReturnsDistinctUsedSupplierNames()
    {
        var dto = BuildOrderDto("Supplier Client", "sup@test.local", null);
        dto.TicketSupplier = "EuroTravel Georgia";
        dto.HotelSupplier = "Hotel Partner";
        await _service.CreateFullOrderAsync(dto);

        var results = await _service.SearchSuppliersAsync("euro", 10);

        Assert.Contains("EuroTravel Georgia", results);
        Assert.DoesNotContain("Hotel Partner", results);
    }

    private static OrderCreateDto BuildOrderDto(
        string fullName,
        string email,
        string? personalNumber)
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        var end = start.AddDays(7);

        return new OrderCreateDto
        {
            Party = new PartyCreateDto
            {
                FullName = fullName,
                Email = email,
                PersonalNumber = personalNumber
            },
            Destination = "Italy",
            StartDate = start,
            EndDate = end,
            PassengerCount = 1,
            Supplier = new SupplierCreateDto { Name = "Test Supplier" },
            Source = "UnitTest",
            SellPriceInGel = 1000,
            Passengers = new()
        };
    }
}
