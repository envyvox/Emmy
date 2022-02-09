using System;
using AutoMapper;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.User.Models
{
    public record UserDto(
        long Id,
        string About,
        uint Level,
        uint Xp,
        Fraction Fraction,
        Location Location,
        Data.Enums.Title Title,
        Gender Gender,
        CubeType CubeType,
        string CommandColor,
        bool IsPremium,
        bool OnGuild,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt);

    public class UserProfile : Profile
    {
        public UserProfile() => CreateMap<Data.Entities.User.User, UserDto>();
    }
}