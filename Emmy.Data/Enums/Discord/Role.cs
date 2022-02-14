using System;
using System.Collections.Generic;

namespace Emmy.Data.Enums.Discord
{
    public enum Role : byte
    {
        Administration = 1,
        EventManager = 2,
        Moderator = 3,

        Premium = 4,
        Streamer = 5,
        NitroBoost = 6, // роль нитро-буста создается дискордом автоматически, нам нужно лишь получить ее
        Creative = 7,
        Friends = 8,

        ContentProvider = 9,
        Active = 10,

        FractionNeutral = 13,
        FractionRedRose = 14,
        FractionWhiteCrow = 15,
        FractionGoldenShark = 16,

        GenderMale = 11,
        GenderFemale = 12,

        GenshinImpact = 17,
        LeagueOfLegends = 18,
        TeamfightTactics = 19,
        Valorant = 20,
        Tarkov = 21,
        DeadByDaylight = 22,
        ApexLegends = 23,
        Dota = 24,
        Minecraft = 25,
        Osu = 26,
        AmongUs = 27,
        Rust = 28,
        CsGo = 29,
        MobileGaming = 30,

        InVoice = 31
    }

    public static class RoleHelper
    {
        public static string Name(this Role role)
        {
            return role switch
            {
                Role.Administration => "Администраторы",
                Role.EventManager => "Организаторы",
                Role.Moderator => "Модераторы",

                Role.Premium => "Премиум",
                Role.Streamer => "Стримеры",
                Role.NitroBoost => "Поддержка сервера",
                Role.Creative => "Креативный вклад",
                Role.Friends => "Друзья проекта",

                Role.ContentProvider => "Поставщик контента",
                Role.Active => "Активные",

                Role.GenderMale => "Оками",
                Role.GenderFemale => "Китсунэ",

                Role.FractionNeutral => Fraction.Neutral.Localize(),
                Role.FractionRedRose => Fraction.RedRose.Localize().Replace("«", "").Replace("»", ""),
                Role.FractionWhiteCrow => Fraction.WhiteCrow.Localize().Replace("«", "").Replace("»", ""),
                Role.FractionGoldenShark => Fraction.GoldenShark.Localize().Replace("«", "").Replace("»", ""),

                Role.GenshinImpact => "Genshin Impact",
                Role.LeagueOfLegends => "League of Legends",
                Role.TeamfightTactics => "Teamfight Tactics",
                Role.Valorant => "Valorant",
                Role.Tarkov => "Escape from Tarkov",
                Role.DeadByDaylight => "Dead by Daylight",
                Role.ApexLegends => "Apex Legends",
                Role.Dota => "Dota 2",
                Role.Minecraft => "Minecraft",
                Role.Osu => "Osu!",
                Role.AmongUs => "Among Us",
                Role.Rust => "Rust",
                Role.CsGo => "CSGO",
                Role.MobileGaming => "Mobile Gaming",

                Role.InVoice => "🎙️",
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };
        }

        public static string Color(this Role role)
        {
            return role switch
            {
                Role.Administration => "ffc7f5",
                Role.EventManager => "e99edb",
                Role.Moderator => "c072b2",
                Role.NitroBoost => "f47fff",
                Role.ContentProvider => "6fffc4",
                Role.Premium => "ffb71d",
                Role.GenderMale => "5ca5f9",
                Role.GenderFemale => "ff7799",
                Role.FractionRedRose => "dd6363",
                Role.FractionWhiteCrow => "87799e",
                Role.FractionGoldenShark => "f3ee4b",
                // для всех остальных используем значение по-умолчанию (прозрачный цвет дискорда)
                _ => "000000"
            };
        }

        public static List<Role> GameRoles()
        {
            return new List<Role>
            {
                Role.GenshinImpact,
                Role.LeagueOfLegends,
                Role.TeamfightTactics,
                Role.Valorant,
                Role.Tarkov,
                Role.DeadByDaylight,
                Role.ApexLegends,
                Role.Dota,
                Role.Minecraft,
                Role.Osu,
                Role.AmongUs,
                Role.Rust,
                Role.CsGo,
                Role.MobileGaming
            };
        }
    }
}