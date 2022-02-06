using System;
using AutoMapper;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.Transit.Models
{
    public record UserMovementDto(
        Location Departure,
        Location Destination,
        DateTimeOffset Arrival);

    public class UserMovementProfile : Profile
    {
        public UserMovementProfile() => CreateMap<UserMovement, UserMovementDto>();
    }
}