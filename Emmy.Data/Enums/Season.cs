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
        public static string Localize(this Season season, bool declension = false)
        {
            return season switch
            {
                Season.Any => declension ? "" : "Любой",
                Season.Spring => declension ? "весны" : "Весна",
                Season.Summer => declension ? "лета" : "Лето",
                Season.Autumn => declension ? "осени" : "Осень",
                Season.Winter => declension ? "зимы" : "Зима",
                _ => throw new ArgumentOutOfRangeException(nameof(season), season, null)
            };
        }

        public static string EmoteName(this Season season)
        {
            return "Season" + season;
        }

        public static Season NextSeason(this Season season)
        {
            return season.GetHashCode() is 4 ? Season.Spring : (Season) season.GetHashCode() + 1;
        }
    }
}