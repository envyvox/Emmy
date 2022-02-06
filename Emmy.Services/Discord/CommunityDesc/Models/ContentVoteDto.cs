using System;
using AutoMapper;
using Emmy.Data.Entities.Discord;
using Emmy.Data.Enums;
using Emmy.Services.Game.User.Models;

namespace Emmy.Services.Discord.CommunityDesc.Models
{
    public record ContentVoteDto(
        Guid Id,
        Vote Vote,
        bool IsActive,
        UserDto User,
        ContentMessageDto ContentMessage);

    public class ContentVoteProfile : Profile
    {
        public ContentVoteProfile() => CreateMap<ContentVote, ContentVoteDto>();
    }
}