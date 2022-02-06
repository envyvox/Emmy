using System;
using AutoMapper;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.Transit.Models
{
    public record TransitDto(
        Guid Id,
        Location Departure,
        Location Destination,
        TimeSpan Duration,
        uint Price);

    public class TransitProfile : Profile
    {
        public TransitProfile() => CreateMap<Data.Entities.Transit, TransitDto>();
    }
}