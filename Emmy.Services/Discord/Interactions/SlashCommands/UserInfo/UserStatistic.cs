using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Statistic.Queries;
using Emmy.Services.Game.User.Queries;
using Humanizer;
using MediatR;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    [RequireCommandChannel]
    public class UserStatistic : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public UserStatistic(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "статистика",
            "Просмотр информации о своей активности как на сервере, так и в игровом мире")]
        public async Task UserStatisticTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userStatistics = await _mediator.Send(new GetUserStatisticsQuery(user.Id));

            var userVoiceMinutes = userStatistics.ContainsKey(Statistic.MinutesInVoice)
                ? userStatistics[Statistic.MinutesInVoice].Amount
                : 0;

            var userMessages = userStatistics.ContainsKey(Statistic.Messages)
                ? userStatistics[Statistic.Messages].Amount
                : 0;

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Статистика")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображается вся информация о твоей активности на сервере и в игровом мире на этой неделе:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Награды за активность начисляются каждый **понедельник**, после чего статистика сбрасывается." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Активность в чате",
                    $"**{userMessages}** {_local.Localize(LocalizationCategory.Basic, "Message", userMessages)}",
                    true)
                .AddField("Голосовой онлайн",
                    $"{userVoiceMinutes.Minutes().Humanize(2, new CultureInfo("ru-RU"))}",
                    true)
                .AddEmptyField(true)
                .AddField("Игровая активность", "Временно недоступно");

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}