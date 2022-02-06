using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Seed.Models
{
    public record UserSeedDto(
        SeedDto Seed,
        uint Amount);

    public class UserSeedProfile : Profile
    {
        public UserSeedProfile() => CreateMap<UserSeed, UserSeedDto>();
    }
}