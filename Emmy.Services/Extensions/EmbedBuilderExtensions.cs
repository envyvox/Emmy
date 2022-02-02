using System.Globalization;
using Discord;

namespace Emmy.Services.Extensions
{
    public static class EmbedBuilderExtensions
    {
        public const string DefaultEmbedColor = "36393F";

        public static EmbedBuilder AddEmptyField(this EmbedBuilder builder, bool inline)
        {
            return builder.AddField(StringExtensions.EmptyChar, StringExtensions.EmptyChar, inline);
        }

        public static EmbedBuilder WithDefaultColor(this EmbedBuilder builder)
        {
            return builder.WithColor(new Color(uint.Parse(DefaultEmbedColor, NumberStyles.HexNumber)));
        }

        public static EmbedBuilder WithUserColor(this EmbedBuilder builder, string commandColor)
        {
            return builder.WithColor(new Color(uint.Parse(commandColor, NumberStyles.HexNumber)));
        }
    }
}