using System;
using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Cooldown.Models
{
    public record UserCooldownDto(
        long UserId,
        Data.Enums.Cooldown Type,
        DateTimeOffset Expiration);

    public class UserCooldownProfile : Profile
    {
        public UserCooldownProfile() => CreateMap<UserCooldown, UserCooldownDto>();
    }
}