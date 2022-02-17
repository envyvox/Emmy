using AutoMapper;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;
using Emmy.Services.Game.Seed.Models;

namespace Emmy.Services.Game.Farm.Models
{
    public record UserFarmDto(
        uint Number,
        FieldState State,
        SeedDto Seed,
        uint Progress,
        bool InReGrowth);

    public class UserFarmProfile : Profile
    {
        public UserFarmProfile() => CreateMap<UserFarm, UserFarmDto>();
    }
}