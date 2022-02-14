using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.DailyReward.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    public class DailyReward : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly TimeZoneInfo _timeZoneInfo;

        public DailyReward(
            IMediator mediator,
            TimeZoneInfo timeZoneInfo)
        {
            _mediator = mediator;
            _timeZoneInfo = timeZoneInfo;
        }

        [SlashCommand(
            "ежедневная-награда",
            "Получить награду за ежедневную активность в игровом мире")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var timeNow = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _timeZoneInfo);
            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            if (user.Fraction is Data.Enums.Fraction.Neutral)
            {
                throw new GameUserExpectedException(
                    $"боюсь что будучи {emotes.GetEmote(Data.Enums.Fraction.Neutral.EmoteName())} " +
                    "**нейтралом** ты не сможешь найти ни одного поставщика, готового наградить тебя за ежедневную активность." +
                    "\n\nТебе необходимо заручиться поддержкой фракции, ведь даже простое упоминание их имен открывает множество дверей." +
                    $"\n\n{emotes.GetEmote("Arrow")} Чтобы вступить во фракцию, напиши {emotes.GetEmote("DiscordSlashCommand")} `/фракция`.");
            }

            var hasTodayReward = await _mediator.Send(new CheckUserDailyRewardQuery(
                user.Id, timeNow.DayOfWeek));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ежедневная награда")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "получай награду за ежедневную активность в игровом мире." +
                    $"\n\n{emotes.GetEmote("Arrow")} Ежедневная награда сбрасывается в 00:00 игрового времени.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(user.IsPremium
                    ? Data.Enums.Image.DailyRewardPremium
                    : Data.Enums.Image.DailyReward)));

            var components = new ComponentBuilder()
                .WithButton("Получить награду", $"daily-reward:{user.Id}", disabled: hasTodayReward)
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components));
        }
    }
}