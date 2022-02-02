using System;
using AutoMapper;
using Emmy.Data.Enums;

namespace Emmy.Services.Game.Banner.Models
{
    public record BannerDto(
        Guid Id,
        string Name,
        BannerRarity Rarity,
        uint Price,
        string Url);

    public class BannerProfile : Profile
    {
        public BannerProfile() => CreateMap<Data.Entities.Banner, BannerDto>();
    }
}