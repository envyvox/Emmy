using System;
using AutoMapper;
using Emmy.Services.Game.Seed.Models;

namespace Emmy.Services.Game.Crop.Models
{
    public record CropDto(
        Guid Id,
        string Name,
        uint Price,
        SeedDto Seed);

    public class CropProfile : Profile
    {
        public CropProfile() => CreateMap<Data.Entities.Crop, CropDto>().MaxDepth(3);
    }
}