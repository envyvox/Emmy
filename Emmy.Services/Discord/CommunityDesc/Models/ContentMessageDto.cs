using System;
using AutoMapper;
using Emmy.Data.Entities.Discord;
using Emmy.Services.Game.User.Models;

namespace Emmy.Services.Discord.CommunityDesc.Models
{
    public record ContentMessageDto(
        Guid Id,
        long ChannelId,
        long MessageId,
        UserDto User);

    public class ContentMessageProfile : Profile
    {
        public ContentMessageProfile() => CreateMap<ContentMessage, ContentMessageDto>();
    }
}