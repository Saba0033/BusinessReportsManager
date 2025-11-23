using AutoMapper;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.ValueObjects;

namespace BusinessReportsManager.Application.Mappings;

public class AppProfile : Profile
{
    public AppProfile()
    {
        // 1. Value Object Mapping 
        // THIS IS KEY TO CLEANER CODE
        CreateMap<Money, MoneyDto>();
        CreateMap<MoneyDto, Money>();

        // 2. Party Mappings
        CreateMap<PersonParty, PersonPartyDto>();
        CreateMap<CompanyParty, CompanyPartyDto>();
        CreateMap<CreatePersonPartyDto, PersonParty>();
        CreateMap<CreateCompanyPartyDto, CompanyParty>();


        // 3. Basic Entity Mappings

        CreateMap<Supplier, SupplierDto>();
        CreateMap<CreateSupplierDto, Supplier>();
        CreateMap<Bank, BankDto>();
        CreateMap<CreateBankDto, Bank>();

        // 4. Tour Related Mappings
        CreateMap<Tour, TourDto>();
        CreateMap<Destination, DestinationDto>();
        CreateMap<AirTicket, AirTicketDto>();
        CreateMap<HotelBooking, HotelBookingDto>();
        CreateMap<ExtraService, ExtraServiceDto>();

        // Assuming you want collections mapped from DTO to Entity (remove ignores)
        // If not, keep the ignores, but be sure those properties aren't in the DTO.
        CreateMap<CreateTourDto, Tour>();
        // Optional: If you still need to ignore collections for Create
        // .ForMember(d => d.Destinations, opt => opt.Ignore()) 
        // .ForMember(d => d.AirTickets, opt => opt.Ignore()) 
        // .ForMember(d => d.HotelBookings, opt => opt.Ignore()) 
        // .ForMember(d => d.ExtraServices, opt => opt.Ignore());

        // 5. Order Mappings (Simplified using the Money map)
        CreateMap<Order, OrderDetailsDto>()
            .ForMember(d => d.OwnedByUserEmail, opt => opt.Ignore())
            // SellPrice and TicketSelfCost will map automatically now!
            .ForMember(d => d.PaymentStatus, opt => opt.Ignore())
            .ForMember(d => d.Passengers, opt => opt.MapFrom(s => s.Passengers))
            .ForMember(d => d.Payments, opt => opt.Ignore());

        CreateMap<Order, OrderListItemDto>()
            .ForMember(d => d.Tour, opt => opt.MapFrom(s => s.Tour != null ? s.Tour.Name : ""))
            .ForMember(d => d.OwnedByUserEmail, opt => opt.Ignore())
            // SellPrice will map automatically now!
            .ForMember(d => d.PaymentStatus, opt => opt.Ignore());

        // 6. Passenger Mappings
        CreateMap<Passenger, PassengerDto>();
        CreateMap<CreatePassengerDto, Passenger>();

        // 7. Payment Mappings (Uses the simplified Money map)
        CreateMap<Payment, PaymentDto>()
            // Amount will map automatically now!
            .ForMember(d => d.BankName, opt => opt.MapFrom(s => s.Bank != null ? s.Bank.Name : ""));

        // 8. Exchange Rate Mappings
        CreateMap<ExchangeRate, ExchangeRateDto>();
        CreateMap<CreateExchangeRateDto, ExchangeRate>();
    }
}