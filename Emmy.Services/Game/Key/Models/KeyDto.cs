using AutoMapper;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.Key.Models
{
    public record KeyDto(
        KeyType Type,
        uint Price);

    public class KeyProfile : Profile
    {
        public KeyProfile() => CreateMap<Data.Entities.Key, KeyDto>();
    }
}