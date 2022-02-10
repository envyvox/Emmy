using System;
using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Title.Models
{
    public record UserTitleDto(
        Data.Enums.Title Type,
        DateTimeOffset CreatedAt);

    public class UserTitleProfile : Profile
    {
        public UserTitleProfile() => CreateMap<UserTitle, UserTitleDto>();
    }
}