using System;
using Emmy.Data.Enums.Discord;

namespace Emmy.Data.Enums
{
    public enum Fraction : byte
    {
        Neutral = 0,
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
                Fraction.Neutral => "Нейтрал",
                Fraction.RedRose => "«Алая роза»",
                Fraction.WhiteCrow => "«Белый ворон»",
                Fraction.GoldenShark => "«Золотая акула»",
                _ => throw new ArgumentOutOfRangeException(nameof(fraction), fraction, null)
            };
        }

        public static string EmoteName(this Fraction fraction)
        {
            return "Fraction" + fraction;
        }

        public static Location Location(this Fraction fraction)
        {
            return fraction switch
            {
                Fraction.RedRose => Enums.Location.RedRose,
                Fraction.WhiteCrow => Enums.Location.WhiteCrow,
                Fraction.GoldenShark => Enums.Location.GoldenShark,
                _ => throw new ArgumentOutOfRangeException(nameof(fraction), fraction, null)
            };
        }

        public static Role Role(this Fraction fraction)
        {
            return fraction switch
            {
                Fraction.Neutral => Discord.Role.FractionNeutral,
                Fraction.RedRose => Discord.Role.FractionRedRose,
                Fraction.WhiteCrow => Discord.Role.FractionWhiteCrow,
                Fraction.GoldenShark => Discord.Role.FractionGoldenShark,
                _ => throw new ArgumentOutOfRangeException(nameof(fraction), fraction, null)
            };
        }

        public static string Description(this Fraction fraction)
        {
            return fraction switch
            {
                Fraction.RedRose =>
                    "Сформировав крепкие связи благодаря своим любовным гостиницам, розы способны убедить любого в своей правоте. Никогда не знаешь через кого они выходят на нужных людей, однако своих целей они достигают быстро и красиво.",
                Fraction.WhiteCrow =>
                    "Отшельники, предпочитающие находится подальше от шумной **Нейтральной зоны**, и проворачивать свои дела без лишних глаз. Не ведут никаких дел с другими фракциями и нейтралами, благодаря чему о них практически ничего не известно.",
                Fraction.GoldenShark =>
                    "Шумная жизнь, огромное количество денег и пропорционально растущее недоверие ко всем вокруг. Если бы не деньги, никто не стал бы сотрудничать с акулами, однако деньги есть деньги.",
                _ => throw new ArgumentOutOfRangeException(nameof(fraction), fraction, null)
            };
        }
    }
}