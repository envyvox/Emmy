﻿using System;
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
        FieldWatering = 7
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
                Location.FieldWatering => declension ? "поливке участка земли" : "Поливка участка земли",
                _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
            };
        }
    }
}