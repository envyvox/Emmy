using System;
using AutoMapper;
using Emmy.Data.Enums;
using Emmy.Services.Game.Crop.Models;

namespace Emmy.Services.Game.Seed.Models
{
    public record SeedDto(
        Guid Id,
        string Name,
        Season Season,
        uint GrowthDays,
        uint ReGrowthDays,
        bool IsMultiply,
        uint Price,
        CropDto Crop);

    public class SeedProfile : Profile
    {
        public SeedProfile() => CreateMap<Data.Entities.Seed, SeedDto>().MaxDepth(3);
    }
}