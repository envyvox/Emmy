using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Referral.Commands;
using Emmy.Services.Game.Referral.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.Referral
{
    public class ReferralSet : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ReferralSet(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "пригласил",
            "Укажи пользователя который тебя пригласил и получи награду")]
        public async Task Execute(
            [Summary("пользователь", "Пользователь который тебя пригласил")]
            IUser mentionedUser)
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var tUser = await _mediator.Send(new GetUserQuery((long) mentionedUser.Id));
            var tSocketUser = await _mediator.Send(new GetSocketGuildUserQuery(mentionedUser.Id));

            if (user.Id == tUser.Id)
            {
                throw new GameUserExpectedException(
                    "ты не можешь указать самого себя как пригласившего тебя пользователя.");
            }

            if (tSocketUser!.IsBot)
            {
                throw new GameUserExpectedException(
                    "ты не можешь указать бота как пригласившего тебя пользователя.");
            }

            var hasReferrer = await _mediator.Send(new CheckUserHasReferrerQuery(user.Id));

            if (hasReferrer)
            {
                var rUser = await _mediator.Send(new GetUserReferrerQuery(user.Id));
                var rSocketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) rUser.Id));

                throw new GameUserExpectedException(
                    $"ты уже указал {rSocketUser?.Mention.AsGameMention(rUser.Title)} " +
                    "как пригласившего тебя пользователя.");
            }

            await _mediator.Send(new CreateUserReferrerCommand(user.Id, tUser.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Реферальная система")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно указал {tSocketUser.Mention.AsGameMention(tUser.Title)} " +
                    $"как пригласившего тебя пользователя и получил {emotes.GetEmote(Container.Token.EmoteName())} " +
                    $"{_local.Localize(LocalizationCategory.Container, Container.Token.ToString())}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Referral)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}