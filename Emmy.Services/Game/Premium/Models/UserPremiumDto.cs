using System;
using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Premium.Models
{
    public record UserPremiumDto(
        Guid Id,
        long UserId,
        DateTimeOffset Expiration,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt);

    public class UserPremiumProfile : Profile
    {
        public UserPremiumProfile() => CreateMap<UserPremium, UserPremiumDto>();
    }
}