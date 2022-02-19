using System;
using Emmy.Data.Enums.Discord;

namespace Emmy.Data.Enums
{
    public enum Location : byte
    {
        InTransit = 0,
        Neutral = 1,
        RedRose = 2,
        WhiteCrow = 3,
        GoldenShark = 4,
        Fishing = 5,
        WorkOnContract = 6,
        FarmWatering = 7
    }

    public static class LocationHelper
    {
        public static string Localize(this Location location, bool declension = false)
        {
            return location switch
            {
                Location.Neutral => declension ? "Нейтральной зоне" : "Нейтральная зона",
                Location.RedRose => declension ? "Угуисидане" : "Угуисудани", // todo добавить название
                Location.WhiteCrow => declension ? "Андеграунде" : "Андеграунд",
                Location.GoldenShark => declension ? "Ясуде дзайбацу" : "Ясуда дзайбацу",
                Location.Fishing => declension ? "рыбалке" : "Рыбалка",
                Location.WorkOnContract => declension
                    ? "."
                    : "..", // Вместо названия локации выводится название контракта
                Location.FarmWatering => declension ? "поливке фермы" : "Поливка фермы",
                _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
            };
        }

        public static string EmoteName(this Location location)
        {
            return location switch {
                Location.InTransit => "InTransit",
                Location.Neutral => Fraction.Neutral.EmoteName(),
                Location.RedRose => Fraction.RedRose.EmoteName(),
                Location.WhiteCrow => Fraction.WhiteCrow.EmoteName(),
                Location.GoldenShark => Fraction.GoldenShark.EmoteName(),
                Location.Fishing => "Fishing",
                Location.WorkOnContract => "WorkOnContract",
                Location.FarmWatering => "FarmWatering",
                _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
            };
        }
    }
}