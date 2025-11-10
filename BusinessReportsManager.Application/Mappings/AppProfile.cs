using AutoMapper;
using BusinessReportsManager.Application.DTOs;
using BusinessReportsManager.Domain.Entities;
using BusinessReportsManager.Domain.ValueObjects;

namespace BusinessReportsManager.Application.Mappings;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<PersonParty, PersonPartyDto>();
        CreateMap<CompanyParty, CompanyPartyDto>();
        CreateMap<CreatePersonPartyDto, PersonParty>();
        CreateMap<CreateCompanyPartyDto, CompanyParty>();
 
        CreateMap<Supplier, SupplierDto>();
        CreateMap<CreateSupplierDto, Supplier>();
        CreateMap<Bank, BankDto>();
        CreateMap<CreateBankDto, Bank>();

        CreateMap<Tour, TourDto>();
        CreateMap<Destination, DestinationDto>();
        CreateMap<AirTicket, AirTicketDto>();
        CreateMap<HotelBooking, HotelBookingDto>();
        CreateMap<ExtraService, ExtraServiceDto>();
        CreateMap<CreateTourDto, Tour>()
            .ForMember(d => d.Destinations, opt => opt.Ignore())
            .ForMember(d => d.AirTickets, opt => opt.Ignore())
            .ForMember(d => d.HotelBookings, opt => opt.Ignore())
            .ForMember(d => d.ExtraServices, opt => opt.Ignore());

        CreateMap<Order, OrderDetailsDto>()
            .ForMember(d => d.OwnedByUserEmail, opt => opt.Ignore())
            .ForMember(d => d.SellPrice, opt => opt.MapFrom(s => new MoneyDto(s.SellPrice.Amount, s.SellPrice.Currency)))
            .ForMember(d => d.TicketSelfCost, opt => opt.MapFrom(s => new MoneyDto(s.TicketSelfCost.Amount, s.TicketSelfCost.Currency)))
            .ForMember(d => d.PaymentStatus, opt => opt.Ignore())
            .ForMember(d => d.Passengers, opt => opt.MapFrom(s => s.Passengers))
            .ForMember(d => d.Payments, opt => opt.Ignore());
        CreateMap<Order, OrderListItemDto>()

            .ForMember(d => d.Tour, opt => opt.MapFrom(s => s.Tour != null ? s.Tour.Name : ""))
            .ForMember(d => d.OwnedByUserEmail, opt => opt.Ignore())
            .ForMember(d => d.SellPrice, opt => opt.MapFrom(s => new MoneyDto(s.SellPrice.Amount, s.SellPrice.Currency)))
            .ForMember(d => d.PaymentStatus, opt => opt.Ignore());

        CreateMap<Passenger, PassengerDto>();
        CreateMap<CreatePassengerDto, Passenger>();

        CreateMap<Payment, PaymentDto>()
            .ForMember(d => d.Amount, opt => opt.MapFrom(s => new MoneyDto(s.Amount.Amount, s.Amount.Currency)))
            .ForMember(d => d.BankName, opt => opt.MapFrom(s => s.Bank != null ? s.Bank.Name : ""));

        CreateMap<ExchangeRate, ExchangeRateDto>();
        CreateMap<CreateExchangeRateDto, ExchangeRate>();

        CreateMap<MoneyDto, Money>();
    }
}