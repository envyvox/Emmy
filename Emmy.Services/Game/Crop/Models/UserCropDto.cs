using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Crop.Models
{
    public record UserCropDto(
        CropDto Crop,
        uint Amount);

    public class UserCropProfile : Profile
    {
        public UserCropProfile() => CreateMap<UserCrop, UserCropDto>();
    }
}