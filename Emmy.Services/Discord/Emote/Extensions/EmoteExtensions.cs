using System.Collections.Generic;
using System.Linq;
using Emmy.Services.Discord.Emote.Models;

namespace Emmy.Services.Discord.Emote.Extensions
{
    public static class EmoteExtensions
    {
        /// <summary> Возвращает код иконки по названию, либо код иконки Blank, если первая не найдена. </summary>
        public static string GetEmote(this Dictionary<string, EmoteDto> emotes, string emoteName)
        {
            return emotes.TryGetValue(emoteName, out var value)
                ? value.Code
                : emotes.TryGetValue("Blank", out var blankValue)
                    ? blankValue.Code
                    : "<:Blank:813150566695174204>";
        }

        /// <summary> Возвращает строку с иконками, отображающими прогресс-бар на основе заданнного числа. </summary>
        public static string DisplayProgressBar(this Dictionary<string, EmoteDto> emotes, uint number)
        {
            var bars = new Dictionary<uint, string>
            {
                {
                    0,
                    $"{emotes.GetEmote("RedCutStart")}{emotes.GetEmote("RedCutEnd")}"
                },
                {
                    10,
                    $"{emotes.GetEmote("RedStart")}{emotes.GetEmote("RedEnd")}"
                },
                {
                    20,
                    $"{emotes.GetEmote("RedStart")}{emotes.GetEmote("RedFull")}{emotes.GetEmote("RedEnd")}"
                },
                {
                    30,
                    $"{emotes.GetEmote("RedStart")}{emotes.GetEmote("RedFull")}{emotes.GetEmote("RedFull")}{emotes.GetEmote("RedEnd")}"
                },
                {
                    40,
                    $"{emotes.GetEmote("YellowStart")}{emotes.GetEmote("YellowFull")}{emotes.GetEmote("YellowFull")}{emotes.GetEmote("YellowEnd")}"
                },
                {
                    50,
                    $"{emotes.GetEmote("YellowStart")}{emotes.GetEmote("YellowFull")}{emotes.GetEmote("YellowFull")}{emotes.GetEmote("YellowFull")}{emotes.GetEmote("YellowEnd")}"
                },
                {
                    60,
                    $"{emotes.GetEmote("YellowStart")}{emotes.GetEmote("YellowFull")}{emotes.GetEmote("YellowFull")}{emotes.GetEmote("YellowFull")}{emotes.GetEmote("YellowFull")}{emotes.GetEmote("YellowEnd")}"
                },
                {
                    70,
                    $"{emotes.GetEmote("GreenStart")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenEnd")}"
                },
                {
                    80,
                    $"{emotes.GetEmote("GreenStart")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenEnd")}"
                },
                {
                    90,
                    $"{emotes.GetEmote("GreenStart")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenEnd")}"
                },
                {
                    100,
                    $"{emotes.GetEmote("GreenStart")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenFull")}{emotes.GetEmote("GreenEnd")}"
                }
            };

            return bars[bars.Keys.Where(x => x <= number).Max()];
        }
    }
}