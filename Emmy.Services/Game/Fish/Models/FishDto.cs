using System;
using System.Collections.Generic;
using AutoMapper;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.Fish.Models
{
    public record FishDto(
        Guid Id,
        string Name,
        FishRarity Rarity,
        Weather CatchWeather,
        TimesDayType CatchTimesDay,
        List<Season> CatchSeasons,
        uint Price);

    public class FishProfile : Profile
    {
        public FishProfile() => CreateMap<Data.Entities.Fish, FishDto>();
    }
}