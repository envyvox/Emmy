using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Premium.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.UserPremium
{
    public class UserPremiumBuy : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public UserPremiumBuy(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("user-premium-buy:*")]
        public async Task Execute(string daysString)
        {
            await Context.Interaction.DeferAsync();

            var days = uint.Parse(daysString);

            var premium30daysPrice = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.PremiumPrice30days));
            var premium365daysPrice = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.PremiumPrice365days));

            var price = days is 30 ? premium30daysPrice : premium365daysPrice;

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, Currency.Lobbs));

            if (userCurrency.Amount < price)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(Currency.Lobbs.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), 5)} " +
                    $"для приобретения статуса {emotes.GetEmote("Premium")} премиум на {days} дней.");
            }

            await _mediator.Send(new RemoveCurrencyFromUserCommand(user.Id, Currency.Lobbs, price));
            await _mediator.Send(new AddUserPremiumCommand(user.Id, TimeSpan.FromDays(days)));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Премиум", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно {(user.IsPremium ? "продлил" : "приобрел")} статус {emotes.GetEmote("Premium")} премиум на {days} дней за " +
                    $"{emotes.GetEmote(Currency.Lobbs.ToString())} {price} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), price)}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.GetPremium)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}