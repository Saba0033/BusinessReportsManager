// using AutoMapper;
// using BusinessReportsManager.Application.AbstractServices;
// using BusinessReportsManager.Application.DTOs;
// using BusinessReportsManager.Application.Mappings;
// using BusinessReportsManager.Domain.Entities;
// using BusinessReportsManager.Domain.Enums;
// using BusinessReportsManager.Domain.Interfaces;
// using BusinessReportsManager.Domain.ValueObjects;
// using BusinessReportsManager.Infrastructure.DataAccess;
// using BusinessReportsManager.Infrastructure.Services;
// using Microsoft.EntityFrameworkCore;
// using Moq;
// using Xunit;
//
// public class OrderServiceTests
// {
//     private readonly DbContextOptions<AppDbContext> _dbOptions;
//     private readonly IMapper _mapper;
//
//     private readonly Mock<IOrderNumberGenerator> _numberGen = new();
//     private readonly Mock<IExchangeRateService> _rates = new();
//     private readonly Mock<IUserService> _users = new();
//
//     public OrderServiceTests()
//     {
//         _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;
//
//         var mapperConfig = new MapperConfiguration(cfg =>
//         {
//             cfg.AddProfile(new AppProfile());
//
//             // DateOnly <-> DateTime conversions if needed anywhere
//             cfg.CreateMap<DateOnly, DateTime>()
//                 .ConvertUsing(d => d.ToDateTime(TimeOnly.MinValue));
//             cfg.CreateMap<DateTime, DateOnly>()
//                 .ConvertUsing(d => DateOnly.FromDateTime(d));
//         });
//
//         _mapper = mapperConfig.CreateMapper();
//     }
//
//     private OrderService CreateService(AppDbContext db)
//     {
//         var repo = new GenericRepository(db);
//         return new OrderService(repo, _mapper, _numberGen.Object, _rates.Object, _users.Object);
//     }
//
//     // --------------------------------------------------------------------
//     // SIMPLE CREATE
//     // --------------------------------------------------------------------
//     // [Fact]
//     // public async Task CreateAsync_Should_Create_Order()
//     // {
//     //     // Arrange
//     //     using var db = new AppDbContext(_dbOptions);
//     //     var service = CreateService(db);
//     //
//     //     var party = new PersonParty
//     //     {
//     //         Id = Guid.NewGuid(),
//     //         FirstName = "John",
//     //         LastName = "Doe",
//     //         Email = "test@aaa.com"
//     //     };
//     //
//     //     var tour = new Tour
//     //     {
//     //         Id = Guid.NewGuid(),
//     //         Name = "Paris Trip",
//     //         StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
//     //         EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
//     //         PassengerCount = 5,
//     //         TourSupplierId = Guid.NewGuid()
//     //     };
//     //
//     //     db.Add(party);
//     //     db.Add(tour);
//     //     await db.SaveChangesAsync();
//     //
//     //     _numberGen.Setup(x => x.NextOrderNumberAsync(default))
//     //         .ReturnsAsync("ORD-001");
//     //
//     //     var dto = new CreateOrderDto(
//     //         OrderPartyId: party.Id,
//     //         TourId: tour.Id,
//     //         Source: "Online",
//     //         SellPrice: new MoneyDto(1200, Currency.USD),
//     //         TicketSelfCost: new MoneyDto(800, Currency.USD)
//     //     );
//     //
//     //     // Act
//     //     var id = await service.CreateAsync(dto, "user123");
//     //
//     //     // Assert
//     //     var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
//     //     Assert.NotNull(order);
//     //     Assert.Equal("ORD-001", order!.OrderNumber);
//     //     Assert.Equal("Online", order.Source);
//     //     Assert.Equal("user123", order.OwnedByUserId);
//     //     Assert.Equal(1200, order.SellPrice.Amount);
//     //     Assert.Equal(OrderStatus.Open, order.Status);
//     // }
//
//     // --------------------------------------------------------------------
//     // FULL CREATE
//     // --------------------------------------------------------------------
//     [Fact]
//     public async Task CreateFullAsync_Should_Create_Order_With_Party_Tour_Passengers_Payments()
//     {
//         // Arrange
//         using var db = new AppDbContext(_dbOptions);
//         var service = CreateService(db);
//
//         var supplierId = Guid.NewGuid();
//         var bankId = Guid.NewGuid();
//
//         db.Add(new Supplier
//         {
//             Id = supplierId,
//             Name = "Test Supplier"
//         });
//
//         db.Add(new Bank
//         {
//             Id = bankId,
//             Name = "TBC"
//         });
//
//         await db.SaveChangesAsync();
//
//         _numberGen.Setup(x => x.NextOrderNumberAsync(default))
//             .ReturnsAsync("ORD-FULL-001");
//
//         var dto = new FullCreateOrderDto
//         {
//             PersonParty = new CreatePersonPartyDto(
//                 Email: "john@example.com",
//                 Phone: "+995598000000",
//                 FirstName: "John",
//                 LastName: "Doe",
//                 BirthDate: DateOnly.FromDateTime(new DateTime(1990, 5, 10))
//             ),
//             CompanyParty = null,
//             Tour = new CreateTourDto(
//                 Name: "Italy Vacation",
//                 StartDate: DateOnly.FromDateTime(new DateTime(2025, 4, 10)),
//                 EndDate: DateOnly.FromDateTime(new DateTime(2025, 4, 17)),
//                 PassengerCount: 2,
//                 Destinations: new List<DestinationDto>
//                 {
//                     new DestinationDto(Guid.Empty, "Italy", "Rome"),
//                     new DestinationDto(Guid.Empty, "Italy", "Venice")
//                 }
//             ),
//             Source = "Office",
//             SellPrice = new MoneyDto(1500, Currency.EUR),
//             TicketSelfCost = new MoneyDto(1000, Currency.EUR),
//             Passengers = new List<CreatePassengerDto>
//             {
//                 new CreatePassengerDto("Anna", "Doe", true, DateOnly.FromDateTime(new DateTime(1992, 8, 14)), "AB123"),
//                 new CreatePassengerDto("Mike", "Doe", false, DateOnly.FromDateTime(new DateTime(2018, 1, 1)), "CD456")
//             },
//             Payments = new List<CreatePaymentDto>
//             {
//                 new CreatePaymentDto(
//                     Amount: new MoneyDto(500, Currency.EUR),
//                     BankId: bankId,
//                     PaidDate: DateOnly.FromDateTime(new DateTime(2025, 3, 1)),
//                     Reference: "PAY12345"
//                 )
//             }
//         };
//
//         // Act
//         var orderId = await service.CreateFullAsync(dto, "owner123");
//
//         // Assert
//         var order = await db.Orders
//             .Include(o => o.OrderParty)
//             .Include(o => o.Tour).ThenInclude(t => t.Destinations)
//             .Include(o => o.Passengers)
//             .Include(o => o.Payments)
//             .FirstOrDefaultAsync(o => o.Id == orderId);
//
//         Assert.NotNull(order);
//         Assert.Equal("ORD-FULL-001", order!.OrderNumber);
//         Assert.Equal("Office", order.Source);
//         Assert.Equal("owner123", order.OwnedByUserId);
//         Assert.Equal(1500, order.SellPrice.Amount);
//         Assert.Equal(2, order.Tour!.PassengerCount);
//         Assert.Equal(2, order.Passengers.Count);
//         Assert.Single(order.Payments);
//     }
//
//     // You can also keep/add tests for GetAsync, GetPagedAsync, AddPassengerAsync, DeletePassengerAsync,
//     // AddPaymentAsync, DeletePaymentAsync as in the previous answer if you want full coverage.
// }
