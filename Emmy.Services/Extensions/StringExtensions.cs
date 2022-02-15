using System.Collections.Generic;
using System.Linq;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;

namespace Emmy.Services.Extensions
{
    public static class StringExtensions
    {
        /// <summary> Unicode Character “⠀” (U+2800) </summary>
        public const string EmptyChar = "⠀";

        public static string RemoveFromEnd(this string source, int amount)
        {
            return source.Length < amount ? source : source.Remove(source.Length - amount);
        }

        public static string AsGameMention(this string mention, Title title)
        {
            var emotes = DiscordRepository.Emotes;

            return $"{emotes.GetEmote(title.EmoteName())} {title.Localize()} {mention}";
        }

        public static string AsPositionEmote(this int position)
        {
            var emotes = DiscordRepository.Emotes;

            return position switch
            {
                1 => emotes.GetEmote("CupGold"),
                2 => emotes.GetEmote("CupSilver"),
                3 => emotes.GetEmote("CupBronze"),
                _ => emotes.GetEmote("List")
            };
        }

        public static string AsLevelEmote(this uint level)
        {
            var emotes = DiscordRepository.Emotes;
            var dict = new Dictionary<uint, string>
            {
                {1, emotes.GetEmote("Level1")},
                {5, emotes.GetEmote("Level5")},
                {10, emotes.GetEmote("Level10")},
                {20, emotes.GetEmote("Level20")},
                {30, emotes.GetEmote("Level30")},
                {50, emotes.GetEmote("Level50")},
                {80, emotes.GetEmote("Level80")},
                {100, emotes.GetEmote("Level100")}
            };

            return dict[dict.Keys.Where(x => x <= level).Max()];
        }
    }
}