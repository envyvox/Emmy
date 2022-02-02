using System;

namespace Emmy.Data.Enums
{
    public enum TimesDayType : byte
    {
        Any = 0,
        Day = 1,
        Night = 2
    }

    public static class TimesDayHelper
    {
        public static string Localize(this TimesDayType timesDay) => timesDay switch
        {
            TimesDayType.Any => "любое",
            TimesDayType.Day => "день",
            TimesDayType.Night => "ночь",
            _ => throw new ArgumentOutOfRangeException(nameof(timesDay), timesDay, null)
        };
    }
}