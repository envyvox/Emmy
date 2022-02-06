using System;
using Emmy.Data.Enums.Discord;

namespace Emmy.Data.Enums
{
    public enum Location : byte
    {
        Neutral = 1,
        RedRose = 2,
        WhiteCrow = 3,
        GoldenShark = 4,
        Fishing = 5,
        WorkOnContract = 6,
        FieldWatering = 7,
    }

    public static class LocationHelper
    {
        public static string Localize(this Location location, bool declension = false)
        {
            return location switch
            {
                Location.Neutral => declension ? "нейтральной зоне" : "Нейтральная зона",
                Location.RedRose => declension ? "..." : "..", // todo добавить название
                Location.WhiteCrow => declension ? "андеграунде" : "Андеграунд",
                Location.GoldenShark => declension ? "ясуде дзайбацу" : "Ясуда дзайбацу",
                Location.Fishing => declension ? "рыбалке" : "Рыбалка",
                Location.WorkOnContract => declension
                    ? "."
                    : "..", // Вместо названия локации выводится название контракта
                Location.FieldWatering => declension ? "поливке участка земли" : "Поливка участка земли",
                _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
            };
        }

        public static Role Role(this Location location)
        {
            return location switch
            {
                Location.Neutral => Discord.Role.LocationNeutral,
                Location.RedRose => Discord.Role.LocationRedRose,
                Location.WhiteCrow => Discord.Role.LocationWhiteCrow,
                Location.GoldenShark => Discord.Role.LocationGoldenShark,
                _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
            };
        }
    }
}