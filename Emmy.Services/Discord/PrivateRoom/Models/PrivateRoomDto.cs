using System;
using AutoMapper;
using Emmy.Services.Game.User.Models;

namespace Emmy.Services.Discord.PrivateRoom.Models
{
    public record PrivateRoomDto(
        Guid Id,
        UserDto User,
        long ChannelId,
        DateTimeOffset Expiration,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt);

    public class FamilyRoomProfile : Profile
    {
        public FamilyRoomProfile() => CreateMap<Data.Entities.Discord.PrivateRoom, PrivateRoomDto>();
    }
}