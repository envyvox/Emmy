using System;
using AutoMapper;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.Achievement.Models
{
    public record AchievementDto(
        Data.Enums.Achievement Type,
        AchievementCategory Category,
        string Name,
        AchievementRewardType RewardType,
        uint RewardNumber,
        DateTimeOffset CreatedAt);

    public class AchievementProfile : Profile
    {
        public AchievementProfile() => CreateMap<Data.Entities.Achievement, AchievementDto>();
    }
}