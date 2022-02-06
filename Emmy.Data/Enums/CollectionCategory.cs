using System;

namespace Emmy.Data.Enums
{
    public enum CollectionCategory : byte
    {
        Crop = 1,
        Fish = 2
    }

    public static class CollectionHelper
    {
        public static string Localize(this CollectionCategory category) => category switch
        {
            CollectionCategory.Crop => "Урожай",
            CollectionCategory.Fish => "Рыба",
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }
}