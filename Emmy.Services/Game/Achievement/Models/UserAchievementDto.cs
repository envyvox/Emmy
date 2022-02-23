using System;
using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Achievement.Models
{
    public record UserAchievementDto(
        AchievementDto Achievement,
        DateTimeOffset CreatedAt);

    public class UserAchievementProfile : Profile
    {
        public UserAchievementProfile() => CreateMap<UserAchievement, UserAchievementDto>();
    }
}