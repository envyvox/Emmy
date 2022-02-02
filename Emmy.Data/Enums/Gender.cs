using System;
using Emmy.Data.Enums.Discord;

namespace Emmy.Data.Enums
{
    public enum Gender : byte
    {
        None = 0,
        Male = 1,
        Female = 2
    }

    public static class GenderHelper
    {
        public static string Localize(this Gender gender)
        {
            return gender switch
            {
                Gender.None => "Не подтвержден",
                Gender.Male => "Мужской",
                Gender.Female => "Женский",
                _ => throw new ArgumentOutOfRangeException(nameof(gender), gender, null)
            };
        }

        public static string EmoteName(this Gender gender)
        {
            return "Gender" + gender;
        }

        public static Role Role(this Gender gender)
        {
            return gender switch
            {
                Gender.Male => Discord.Role.GenderMale,
                Gender.Female => Discord.Role.GenderFemale,
                _ => throw new ArgumentOutOfRangeException(nameof(gender), gender, null)
            };
        }
    }
}