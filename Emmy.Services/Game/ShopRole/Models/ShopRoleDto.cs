using AutoMapper;

namespace Emmy.Services.Game.ShopRole.Models
{
    public record ShopRoleDto(
        long RoleId,
        uint Price);

    public class ShopRoleProfile : Profile
    {
        public ShopRoleProfile() => CreateMap<Data.Entities.ShopRole, ShopRoleDto>();
    }
}