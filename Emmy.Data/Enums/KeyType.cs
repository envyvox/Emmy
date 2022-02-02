using System;

namespace Emmy.Data.Enums
{
    public enum KeyType : byte
    {
        LoveRoom = 1,
        PrivateRoom = 2
    }

    public static class KeyHelper
    {
        public static string EmoteName(this KeyType type)
        {
            return "Key" + type;
        }

        public static string Description(this KeyType type)
        {
            return type switch
            {
                KeyType.LoveRoom => "Открывает доступ к созданию любовного гнезда на 30 дней",
                KeyType.PrivateRoom => "Открывает доступ к созданию приватного сектора на 30 дней",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}