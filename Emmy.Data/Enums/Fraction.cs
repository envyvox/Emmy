using System;

namespace Emmy.Data.Enums
{
    public enum Fraction : byte
    {
        Undefined = 0,
        RedRose = 1,
        WhiteCrow = 2,
        GoldenShark = 3
    }

    public static class FractionHelper
    {
        public static string Localize(this Fraction fraction)
        {
            return fraction switch
            {
                Fraction.Undefined => "Не выбрана",
                Fraction.RedRose => "«Алая роза»",
                Fraction.WhiteCrow => "«Белая ворона»",
                Fraction.GoldenShark => "«Золотая акула»",
                _ => throw new ArgumentOutOfRangeException(nameof(fraction), fraction, null)
            };
        }

        public static string EmoteName(this Fraction fraction)
        {
            return "Fraction" + fraction;
        }
    }
}