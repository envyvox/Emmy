using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Statistic.Models
{
    public record UserStatisticDto(
        Data.Enums.Statistic Type,
        uint Amount);

    public class UserStatisticProfile : Profile
    {
        public UserStatisticProfile() => CreateMap<UserStatistic, UserStatisticDto>();
    }
}