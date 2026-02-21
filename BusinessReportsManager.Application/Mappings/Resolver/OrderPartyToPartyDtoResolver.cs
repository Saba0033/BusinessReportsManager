using AutoMapper;
using BusinessReportsManager.Application.DTOs.OrderParty;
using BusinessReportsManager.Domain.Entities;

namespace BusinessReportsManager.Application.Mappings;

public sealed class OrderPartyToPartyDtoResolver : IValueResolver<Order, object, PartyDto>
{
    public PartyDto Resolve(Order source, object destination, PartyDto destMember, ResolutionContext context)
    {
        if (source.OrderParty is PersonParty p)
        {
            return new PartyDto
            {
                Id = p.Id,
                FullName = $"{p.FirstName} {p.LastName}".Trim(),
                Email = p.Email,
                Phone = p.Phone
            };
        }

        // fallback (old orders could still have CompanyParty or null)
        return new PartyDto();
    }
}

