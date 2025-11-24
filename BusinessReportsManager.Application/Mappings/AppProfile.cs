using AutoMapper;
using BusinessReportsManager.Application.DTOs.AirTicket;
using BusinessReportsManager.Application.DTOs.ExtraService;
using BusinessReportsManager.Application.DTOs.HotelBooking;
using BusinessReportsManager.Application.DTOs.Order;
using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Application.DTOs.Passenger;
using BusinessReportsManager.Application.DTOs.Payment;
using BusinessReportsManager.Application.DTOs.PriceCurrency;
using BusinessReportsManager.Application.DTOs.Supplier;
using BusinessReportsManager.Application.DTOs.Tour;
using BusinessReportsManager.Domain.Entities;

namespace BusinessReportsManager.Application.Mappings;

public class AppProfile : Profile
{
    public AppProfile()
    {
        // ======================================================
        // PRICE CURRENCY
        // ======================================================
        CreateMap<PriceCurrency, PriceCurrencyDto>();
        CreateMap<PriceCurrencyCreateDto, PriceCurrency>();

        // ======================================================
        // ORDER PARTY
        // ======================================================
        CreateMap<PersonParty, PersonPartyDto>();
        CreateMap<CompanyParty, CompanyPartyDto>();

        CreateMap<PartyCreateDto, PersonParty>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName))
            .ForMember(d => d.BirthDate, o => o.MapFrom(s => s.BirthDate));

        CreateMap<PartyCreateDto, CompanyParty>()
            .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.CompanyName))
            .ForMember(d => d.RegistrationNumber, o => o.MapFrom(s => s.RegistrationNumber))
            .ForMember(d => d.ContactPerson, o => o.MapFrom(s => s.ContactPerson));

        // ======================================================
        // SUPPLIER
        // ======================================================
        CreateMap<Supplier, SupplierDto>();
        CreateMap<SupplierCreateDto, Supplier>();

        // ======================================================
        // PASSENGER
        // ======================================================
        CreateMap<Passenger, PassengerDto>();
        CreateMap<PassengerCreateDto, Passenger>();

        // ======================================================
        // PAYMENT
        // ======================================================
        // PaymentDto.Price ← Payment.PriceCurrency
        CreateMap<Payment, PaymentDto>()
            .ForMember(d => d.Price, o => o.MapFrom(s => s.PriceCurrency));

        // PaymentCreateDto.Price → PriceCurrency
        CreateMap<PaymentCreateDto, Payment>()
            .ForMember(d => d.PriceCurrency, o => o.Ignore());

        // ======================================================
        // AIR TICKET
        // ======================================================
        CreateMap<AirTicket, AirTicketDto>()
            .ForMember(d => d.Price, o => o.MapFrom(s => s.PriceCurrency));

        CreateMap<AirTicketCreateDto, AirTicket>()
            .ForMember(d => d.PriceCurrency, o => o.Ignore());

        // ======================================================
        // HOTEL BOOKING
        // ======================================================
        CreateMap<HotelBooking, HotelBookingDto>()
            .ForMember(d => d.Price, o => o.MapFrom(s => s.PriceCurrency));

        CreateMap<HotelBookingCreateDto, HotelBooking>()
            .ForMember(d => d.PriceCurrency, o => o.Ignore());

        // ======================================================
        // EXTRA SERVICE
        // ======================================================
        CreateMap<ExtraService, ExtraServiceDto>()
            .ForMember(d => d.Price, o => o.MapFrom(s => s.PriceCurrency));

        CreateMap<ExtraServiceCreateDto, ExtraService>()
            .ForMember(d => d.PriceCurrency, o => o.Ignore());

        // ======================================================
        // TOUR
        // ======================================================
        CreateMap<Tour, TourDto>()
            .ForMember(d => d.Supplier, o => o.MapFrom(s => s.TourSupplier))
            .ForMember(d => d.Passengers, o => o.MapFrom(s => s.Passengers))
            .ForMember(d => d.AirTickets, o => o.MapFrom(s => s.AirTickets))
            .ForMember(d => d.HotelBookings, o => o.MapFrom(s => s.HotelBookings))
            .ForMember(d => d.ExtraServices, o => o.MapFrom(s => s.ExtraServices));

        CreateMap<TourCreateDto, Tour>()
            .ForMember(d => d.TourSupplier, o => o.Ignore())
            .ForMember(d => d.Passengers, o => o.Ignore())
            .ForMember(d => d.AirTickets, o => o.Ignore())
            .ForMember(d => d.HotelBookings, o => o.Ignore())
            .ForMember(d => d.ExtraServices, o => o.Ignore());

        // ======================================================
        // ORDER
        // ======================================================
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.PersonParty, o => o.MapFrom(s => s.OrderParty as PersonParty))
            .ForMember(d => d.CompanyParty, o => o.MapFrom(s => s.OrderParty as CompanyParty))
            .ForMember(d => d.Tour, o => o.MapFrom(s => s.Tour))
            .ForMember(d => d.Payments, o => o.MapFrom(s => s.Payments));
    }
}
