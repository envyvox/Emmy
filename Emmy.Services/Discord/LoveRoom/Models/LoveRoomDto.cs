using System;
using AutoMapper;
using Emmy.Data.Entities;

namespace Emmy.Services.Discord.LoveRoom.Models
{
    public record LoveRoomDto(
        Guid Id,
        Relationship Relationship,
        long ChannelId,
        DateTimeOffset Expiration,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt);

    public class LoveRoomProfile : Profile
    {
        public LoveRoomProfile() => CreateMap<Data.Entities.Discord.LoveRoom, LoveRoomDto>();
    }
}