using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Container.Models
{
    public record UserContainerDto(
        Data.Enums.Container Type,
        uint Amount);

    public class UserContainerProfile : Profile
    {
        public UserContainerProfile() => CreateMap<UserContainer, UserContainerDto>();
    }
}