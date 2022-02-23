using System;

namespace Emmy.Data.Enums
{
    public enum AchievementCategory : byte
    {
        FirstSteps = 1,
        Fishing = 2,
        Harvesting = 3,
        Casino = 4,
        Trading = 5,
        Fraction = 6,
        Collection = 7
    }

    public static class AchievementCategoryHelper
    {
        public static string Localize(this AchievementCategory category)
        {
            return category switch {
                AchievementCategory.FirstSteps => "Первые шаги",
                AchievementCategory.Fishing => "Рыбалка",
                AchievementCategory.Harvesting => "Выращивание урожая",
                AchievementCategory.Casino => "Казино",
                AchievementCategory.Trading => "Торговля",
                AchievementCategory.Fraction => "Фракция",
                AchievementCategory.Collection => "Коллекция",
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            };
        }
    }
}