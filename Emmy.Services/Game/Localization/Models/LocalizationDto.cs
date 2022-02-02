using System;
using AutoMapper;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.Localization.Models
{
    public record LocalizationDto(
        Guid Id,
        LocalizationCategory Category,
        string Name,
        string Single,
        string Double,
        string Multiply);

    public class LocalizationProfile : Profile
    {
        public LocalizationProfile() => CreateMap<Data.Entities.Localization, LocalizationDto>();
    }
}