using AutoMapper;

namespace Emmy.Services.Game.Fraction.Models
{
    public record FractionDto(
        Data.Enums.Fraction Type,
        uint Points);

    public class FractionProfile : Profile
    {
        public FractionProfile() => CreateMap<Data.Entities.Fraction, FractionDto>();
    }
}