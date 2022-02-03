using System;

namespace Emmy.Data.Enums
{
    public enum FishRarity : byte
    {
        Common = 1,
        Rare = 2,
        Epic = 3,
        Mythical = 4,
        Legendary = 5
    }

    public static class FishRarityHelper
    {
        public static string Localize(this FishRarity rarity, bool declension = false)
        {
            return rarity switch
            {
                FishRarity.Common => declension ? "обычную" : "Обычная",
                FishRarity.Rare => declension ? "редкую" : "Редкая",
                FishRarity.Epic => declension ? "эпическую" : "Эпическая",
                FishRarity.Mythical => declension ? "мифическую" : "Мифическая",
                FishRarity.Legendary => declension ? "легендарную" : "Легендарная",
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }
    }
}