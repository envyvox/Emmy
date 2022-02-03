using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Fish.Models
{
    public record UserFishDto(
        FishDto Fish,
        uint Amount);

    public class UserFishProfile : Profile
    {
        public UserFishProfile() => CreateMap<UserFish, UserFishDto>();
    }
}