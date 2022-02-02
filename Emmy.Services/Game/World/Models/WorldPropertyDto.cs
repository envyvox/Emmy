using AutoMapper;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.World.Models
{
    public record WorldPropertyDto(
        WorldProperty Type,
        uint Value);

    public class WorldPropertyProfile : Profile
    {
        public WorldPropertyProfile() => CreateMap<Data.Entities.WorldProperty, WorldPropertyDto>();
    }
}