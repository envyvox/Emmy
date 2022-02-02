using System;

namespace Emmy.Data.Enums
{
    public enum Weather : byte
    {
        Any = 0,
        Clear = 1,
        Rain = 2
    }

    public static class WeatherHelper
    {
        public static string Localize(this Weather weather)
        {
            return weather switch
            {
                Weather.Any => "любой",
                Weather.Clear => "ясной",
                Weather.Rain => "дождливой",
                _ => throw new ArgumentOutOfRangeException(nameof(weather), weather, null)
            };
        }

        public static string EmoteName(this Weather weather)
        {
            return "Weather" + weather;
        }
    }
}