using System;
using AutoMapper;
using Emmy.Data.Entities.Discord;
using Emmy.Data.Enums;

namespace Emmy.Services.Discord.GenderRequest.Models
{
    public record UserGenderRequestDto(
        Guid Id,
        long UserId,
        RequestState State,
        long? ModeratorId,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt);

    public class UserGenderRequestProfile : Profile
    {
        public UserGenderRequestProfile() => CreateMap<UserGenderRequest, UserGenderRequestDto>();
    }
}