using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Banner.Models
{
    public record UserBannerDto(
        BannerDto Banner,
        bool IsActive);

    public class UserBannerProfile : Profile
    {
        public UserBannerProfile() => CreateMap<UserBanner, UserBannerDto>();
    }
}