using System;

namespace Emmy.Data.Enums
{
    public enum Season : byte
    {
        Any = 0,
        Spring = 1,
        Summer = 2,
        Autumn = 3,
        Winter = 4
    }

    public static class SeasonHelper
    {
        public static string Localize(this Season season)
        {
            return season switch
            {
                Season.Any => "Любой",
                Season.Spring => "Весна",
                Season.Summer => "Лето",
                Season.Autumn => "Осень",
                Season.Winter => "Зима",
                _ => throw new ArgumentOutOfRangeException(nameof(season), season, null)
            };
        }

        public static string EmoteName(this Season season)
        {
            return "Season" + season;
        }
    }
}