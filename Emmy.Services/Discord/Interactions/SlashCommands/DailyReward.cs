using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.DailyReward.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    public class DailyReward : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public DailyReward(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "ежедневная-награда",
            "Получить награду за ежедневную активность в игровом мире")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasTodayReward = await _mediator.Send(new CheckUserDailyRewardQuery(
                user.Id, DateTimeOffset.UtcNow.DayOfWeek));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ежедневная награда")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "получай награду за ежедневную активность в игровом мире." +
                    $"\n\n{emotes.GetEmote("Arrow")} Ежедневная награда сбрасывается в 00:00 игрового времени.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(user.IsPremium
                    ? Data.Enums.Image.DailyReward
                    : Data.Enums.Image.DailyRewardPremium)));

            var components = new ComponentBuilder()
                .WithButton("Получить награду", $"daily-reward:{user.Id}", disabled: hasTodayReward)
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components));
        }
    }
}