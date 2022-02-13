using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using Humanizer.Localisation;
using MediatR;
using static Discord.Emote;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    public class WorldInfo : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly TimeZoneInfo _timeZoneInfo;

        public WorldInfo(
            IMediator mediator,
            TimeZoneInfo timeZoneInfo)
        {
            _mediator = mediator;
            _timeZoneInfo = timeZoneInfo;
        }

        [SlashCommand(
            "мир",
            "Информация о текущем состоянии мира")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var timeNow = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _timeZoneInfo);
            var currentSeason = await _mediator.Send(new GetCurrentSeasonQuery());
            var weatherToday = await _mediator.Send(new GetWeatherTodayQuery());
            var weatherTomorrow = await _mediator.Send(new GetWeatherTomorrowQuery());
            var currentTimesDay = await _mediator.Send(new GetCurrentTimesDayQuery());

            var daysInMonth = DateTime.DaysInMonth(timeNow.Year, timeNow.Month);
            var daysBeforeNewMonth = daysInMonth - timeNow.Day;

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Информация о мире")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображается информация о текущем состоянии мира:")
                .AddField(StringExtensions.EmptyChar,
                    $"Сейчас {timeNow.ToString("HH:mm", new CultureInfo("ru-RU"))}, " +
                    $"{emotes.GetEmote(currentTimesDay.ToString())} **{currentTimesDay.Localize()}**")
                .AddField(StringExtensions.EmptyChar,
                    $"Погода сегодня будет {emotes.GetEmote(weatherToday.EmoteName())} **{weatherToday.Localize()}**")
                .AddField(StringExtensions.EmptyChar,
                    $"Погода завтра обещает быть {emotes.GetEmote(weatherTomorrow.EmoteName())} **{weatherTomorrow.Localize()}**")
                .AddField(StringExtensions.EmptyChar,
                    $"Сейчас {emotes.GetEmote(currentSeason.EmoteName())} **{currentSeason.Localize().ToLower()}**, " +
                    $"до наступления {emotes.GetEmote(currentSeason.NextSeason().EmoteName())} " +
                    $"**{currentSeason.NextSeason().Localize(true)}** осталось " +
                    $"**{daysBeforeNewMonth.Days().Humanize(maxUnit: TimeUnit.Day, culture: new CultureInfo("ru-RU"))}**")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.WorldInfo)));

            var components = new ComponentBuilder()
                .WithSelectMenu(new SelectMenuBuilder()
                    .WithPlaceholder("Выбери вопрос который тебя интересует")
                    .WithCustomId("world-info-qa")
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, на что влияет время суток?",
                        "timesDay",
                        emote: Parse(emotes.GetEmote("DiscordHelp")))
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, на что влияет погода?",
                        "weather",
                        emote: Parse(emotes.GetEmote("DiscordHelp")))
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, на что влияет сезон?",
                        "season",
                        emote: Parse(emotes.GetEmote("DiscordHelp"))))
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components));
        }
    }
}