using AutoMapper;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.Key.Models
{
    public record UserKeyDto(
        long UserId,
        KeyType Type,
        uint Amount);

    public class UserKeyProfile : Profile
    {
        public UserKeyProfile() => CreateMap<UserKey, UserKeyDto>();
    }
}