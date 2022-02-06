using System;
using System.Collections.Generic;

namespace Emmy.Data.Enums.Discord
{
    public enum Role : byte
    {
        Administration,
        EventManager,
        Moderator,

        Premium,
        Streamer,
        Nitro, // роль нитро-буста создается дискордом автоматически, нам нужно лишь получить ее
        Creative,
        Friends,

        ContentProvider,
        Active,

        GenderMale,
        GenderFemale,

        LocationNeutral,
        LocationRedRose,
        LocationWhiteCrow,
        LocationGoldenShark,

        GenshinImpact,
        LeagueOfLegends,
        TeamfightTactics,
        Valorant,
        Tarkov,
        DeadByDaylight,
        ApexLegends,
        Dota,
        Minecraft,
        Osu,
        AmongUs,
        Rust,
        CsGo,
        MobileGaming,

        InVoice
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
                Role.Nitro => "Поддержка сервера",
                Role.Creative => "Креативный вклад",
                Role.Friends => "Друзья проекта",

                Role.ContentProvider => "Поставщик контента",
                Role.Active => "Активные",

                Role.GenderMale => "Оками",
                Role.GenderFemale => "Китсунэ",

                Role.LocationNeutral => Location.Neutral.Localize(),
                Role.LocationRedRose => Location.RedRose.Localize(),
                Role.LocationWhiteCrow => Location.WhiteCrow.Localize(),
                Role.LocationGoldenShark => Location.GoldenShark.Localize(),

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
                Role.Nitro => "f47fff",
                Role.ContentProvider => "6fffc4",
                Role.Premium => "ffb71d",
                Role.GenderMale => "5ca5f9",
                Role.GenderFemale => "ff7799",
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