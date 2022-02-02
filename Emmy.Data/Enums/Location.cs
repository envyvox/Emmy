using System;
using Emmy.Data.Enums.Discord;

namespace Emmy.Data.Enums
{
    public enum Location : byte
    {
        Neutral = 1
    }

    public static class LocationHelper
    {
        public static string Localize(this Location location, bool declension = false)
        {
            return location switch
            {
                Location.Neutral => declension ? "нейтральной зоне" : "Нейтральная зона",
                _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
            };
        }

        public static Role Role(this Location location)
        {
            return location switch {
                Location.Neutral => Discord.Role.LocationNeutral,
                _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
            };
        }
    }
}