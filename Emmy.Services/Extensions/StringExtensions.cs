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
    }
}