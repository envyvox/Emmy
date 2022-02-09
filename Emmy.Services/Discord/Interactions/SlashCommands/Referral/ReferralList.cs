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
using Emmy.Services.Game.Referral.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.SlashCommands.Referral
{
    public class ReferralList : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ReferralList(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "приглашения",
            "Просматривай информацию о своем участии в реферальной системе")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userReferrals = await _mediator.Send(new GetUserReferralsQuery(user.Id));
            var hasReferrer = await _mediator.Send(new CheckUserHasReferrerQuery(user.Id));

            string referrerString;
            if (hasReferrer)
            {
                var referrer = await _mediator.Send(new GetUserReferrerQuery(user.Id));
                var socketReferrer = await _mediator.Send(new GetSocketGuildUserQuery((ulong) referrer.Id));

                referrerString =
                    $"Ты указал {socketReferrer?.Mention.AsGameMention(referrer.Title)} " +
                    $"как пригласившего тебя пользователя и получил {emotes.GetEmote(Container.Token.EmoteName())} " +
                    $"{_local.Localize(LocalizationCategory.Container, Container.Token.ToString())}.";
            }
            else
            {
                referrerString =
                    "Ты не указал пользователя который тебя пригласил." +
                    $"\n\n{emotes.GetEmote("Arrow")} Напиши {emotes.GetEmote("DiscordSlashCommand")} `/пригласил`, укажи пользователя и получи " +
                    $"\n{emotes.GetEmote(Container.Token.EmoteName())} " +
                    $"{_local.Localize(LocalizationCategory.Container, Container.Token.ToString())}.";
            }

            var referralString = string.Empty;

            foreach (var userReferral in userReferrals)
            {
                var socketUserReferral = await _mediator.Send(new GetSocketGuildUserQuery((ulong) userReferral.Id));

                referralString +=
                    $"{emotes.GetEmote("List")} {socketUserReferral?.Mention.AsGameMention(userReferral.Title)}\n";
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Реферальная система")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображается информация о твоем участии в реферальной системе:" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Твой реферер",
                    referrerString + $"\n{StringExtensions.EmptyChar}")
                .AddField("Приглашенные пользователи",
                    referralString.Length > 0
                        ? referralString.Length > 1024
                            ? "У тебя так много приглашенных пользователей, что мне трудно назвать их всех! " +
                              $"Но их точно **{userReferrals.Count}**"
                            : referralString
                        : "Ты еще не пригласил ни одного пользователя.\nПриглашай друзей и получайте " +
                          $"{emotes.GetEmote(Container.Token.EmoteName())} {emotes.GetEmote(Currency.Lobbs.ToString())} " +
                          "бонусы реферальной системы вместе.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Referral)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Узнать о наградах реферальной системы",
                    "referral-rewards",
                    ButtonStyle.Secondary,
                    Parse(emotes.GetEmote("DiscordHelp")))
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components));
        }
    }
}