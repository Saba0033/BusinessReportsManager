using AutoMapper;
using BusinessReportsManager.Application.DTOs;
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
using System;
using System.Collections.Generic;
using System.Linq;

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
        // ORDER PARTY (Person only; API exposes PartyDto)
        // ======================================================
        CreateMap<PersonParty, PartyDto>()
            .ForMember(d => d.FullName,
                o => o.MapFrom(s => $"{s.FirstName} {s.LastName}".Trim()));

        CreateMap<PartyCreateDto, PersonParty>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace(s.FullName)
                    ? string.Empty
                    : s.FullName.Trim()
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]
            ))
            .ForMember(d => d.LastName, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace(s.FullName)
                    ? string.Empty
                    : string.Join(" ",
                        s.FullName.Trim()
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Skip(1))
            ))
            .ForMember(d => d.BirthDate, o => o.Ignore());

        // ======================================================
        // SUPPLIER
        // ======================================================
        CreateMap<Supplier, SupplierDto>();
        CreateMap<SupplierCreateDto, Supplier>();

        // ======================================================
        // PASSENGER (FullName only in DTO)
        // ======================================================
        CreateMap<Passenger, PassengerDto>()
            .ForMember(d => d.FullName,
                o => o.MapFrom(s => $"{s.FirstName} {s.LastName}".Trim()));

        CreateMap<PassengerCreateDto, Passenger>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace(s.FullName)
                    ? string.Empty
                    : s.FullName.Trim()
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]
            ))
            .ForMember(d => d.LastName, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace(s.FullName)
                    ? string.Empty
                    : string.Join(" ",
                        s.FullName.Trim()
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Skip(1))
            ))
            .ForMember(d => d.BirthDate, o => o.Ignore())
            .ForMember(d => d.DocumentNumber, o => o.Ignore());

        // ======================================================
        // PAYMENT
        // ======================================================
        CreateMap<Payment, PaymentDto>()
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.PriceCurrency));

        CreateMap<PaymentCreateDto, Payment>()
            .ForMember(d => d.PriceCurrency, o => o.Ignore());

        // ======================================================
        // AIR TICKET
        // ======================================================
        CreateMap<AirTicket, AirTicketDto>()
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.PriceCurrency));

        CreateMap<AirTicketCreateDto, AirTicket>()
            .ForMember(d => d.PriceCurrency, o => o.Ignore());

        // ======================================================
        // HOTEL BOOKING
        // ======================================================
        CreateMap<HotelBooking, HotelBookingDto>()
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.PriceCurrency));

        CreateMap<HotelBookingCreateDto, HotelBooking>()
            .ForMember(d => d.PriceCurrency, o => o.Ignore());

        // ======================================================
        // EXTRA SERVICE (for GET only)
        // ======================================================
        CreateMap<ExtraService, ExtraServiceDto>()
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.PriceCurrency));

        CreateMap<ExtraServiceCreateDto, ExtraService>()
            .ForMember(d => d.PriceCurrency, o => o.Ignore());

        // ======================================================
        // TOUR
        // ======================================================
        CreateMap<Tour, TourDto>()
           .ForMember(d => d.Destination, o => o.MapFrom(s => s.Name))
           .ForMember(d => d.Supplier, o => o.MapFrom(s => s.TourSupplier))
            .ForMember(d => d.Passengers,
                o => o.MapFrom(s => s.Passengers))
            .ForMember(d => d.AirTickets,
                o => o.MapFrom(s => s.AirTickets))
            .ForMember(d => d.HotelBookings,
                o => o.MapFrom(s => s.HotelBookings))
            .ForMember(d => d.ExtraServices,
                o => o.MapFrom(s => s.ExtraServices));

        CreateMap<TourCreateDto, Tour>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Destination))
            .ForMember(d => d.TourSupplier, o => o.Ignore())
            .ForMember(d => d.Passengers, o => o.Ignore())
            .ForMember(d => d.AirTickets, o => o.Ignore())
            .ForMember(d => d.HotelBookings, o => o.Ignore())
            .ForMember(d => d.ExtraServices, o => o.Ignore());

        // ======================================================
        // ORDER (using resolver)
        // ======================================================
        CreateMap<BusinessReportsManager.Domain.Entities.Order, BusinessReportsManager.Application.DTOs.Order.OrderDto>()
            .ForMember(d => d.Party,
                o => o.MapFrom<OrderPartyToPartyDtoResolver>())
            .ForMember(d => d.Tour,
                o => o.MapFrom(s => s.Tour))
            .ForMember(d => d.Payments,
                o => o.MapFrom(s => s.Payments ?? new List<Payment>()))
            .ForMember(d => d.CreatedById,
                o => o.MapFrom(s => s.CreatedById))
            .ForMember(d => d.CreatedByEmail,
                o => o.MapFrom(s => s.CreatedByEmail))
            .ForMember(d => d.AccountingComment, o => o.MapFrom(s => s.AccountingComment))
            .ForMember(d => d.AccountingCommentUpdatedAtUtc, o => o.MapFrom(s => s.AccountingCommentUpdatedAtUtc))
            .ForMember(d => d.AccountingCommentUpdatedById, o => o.MapFrom(s => s.AccountingCommentUpdatedById))
            .ForMember(d => d.AccountingCommentUpdatedByEmail, o => o.MapFrom(s => s.AccountingCommentUpdatedByEmail))
            .ForMember(d => d.TotalExpenseInGel, o => o.MapFrom(s => s.TotalExpenseInGel));

        CreateMap<CustomerBankRequisites, CustomerBankRequisitesDto>();
        CreateMap<CustomerBankRequisitesCreateDto, CustomerBankRequisites>();


    }
}
