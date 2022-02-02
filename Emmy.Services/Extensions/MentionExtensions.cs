using System;

namespace Emmy.Services.Extensions
{
    public enum MentionType : byte
    {
        User = 1,
        Role = 2,
        Channel = 3
    }

    public static class MentionExtensions
    {
        public static string ToMention(this ulong id, MentionType type)
        {
            return type switch
            {
                MentionType.User => $"<@{id}>",
                MentionType.Role => $"<@&{id}>",
                MentionType.Channel => $"<#{id}>",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static string ToMention(this long id, MentionType type)
        {
            return ToMention((ulong) id, type);
        }
    }
}