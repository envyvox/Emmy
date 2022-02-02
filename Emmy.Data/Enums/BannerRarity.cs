using System;

namespace Emmy.Data.Enums
{
    public enum BannerRarity : byte
    {
        Common = 1,
        Rare = 2,
        Animated = 3,
        Limited = 4,
        Custom = 5
    }

    public static class BannerRarityHelper
    {
        public static string Localize(this BannerRarity rarity, bool declension = false)
        {
            return rarity switch
            {
                BannerRarity.Common => declension ? "обычного" : "Обычный",
                BannerRarity.Rare => declension ? "редкого" : "Редкий",
                BannerRarity.Animated => declension ? "анимированного" : "Анимированный",
                BannerRarity.Limited => declension ? "лимитированного" : "Лимитированный",
                BannerRarity.Custom => declension ? "персонального" : "Персональный",
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }

        public static string EmoteName(this BannerRarity rarity)
        {
            return "BannerRarity" + rarity;
        }
    }
}