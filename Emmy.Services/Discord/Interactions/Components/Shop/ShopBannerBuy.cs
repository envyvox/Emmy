using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Banner.Commands;
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Shop
{
    [RequireLocation(Location.Neutral)]
    public class ShopBannerBuy : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopBannerBuy(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("shop-banner-buy-currency:*")]
        public async Task ShopBannerBuyTask(string currencyHashcode, string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var currency = (Currency) int.Parse(currencyHashcode);
            var bannerId = Guid.Parse(selectedValues.First());

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var banner = await _mediator.Send(new GetBannerQuery(bannerId));
            var hasBanner = await _mediator.Send(new CheckUserHasBannerQuery(user.Id, banner.Id));
            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, currency));
            var bannerPrice = currency is Currency.Token ? banner.Price : banner.Price.ConvertTokensToLobbs();

            if (hasBanner)
            {
                throw new GameUserExpectedException(
                    $"у тебя уже есть этот {emotes.GetEmote(banner.Rarity.EmoteName())} " +
                    $"{banner.Rarity.Localize().ToLower()} баннер, зачем тебе покупать его еще раз?");
            }

            if (userCurrency.Amount < bannerPrice)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(currency.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), 5)} " +
                    $"для приобретения {emotes.GetEmote(banner.Rarity.EmoteName())} " +
                    $"{banner.Rarity.Localize(true)} баннера «{banner.Name}».");
            }

            await _mediator.Send(new RemoveCurrencyFromUserCommand(user.Id, currency, bannerPrice));
            await _mediator.Send(new AddBannerToUserCommand(user.Id, banner.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин баннеров")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно приобрел {emotes.GetEmote(banner.Rarity.EmoteName())} " +
                    $"{banner.Rarity.Localize().ToLower()} баннер «{banner.Name}» за " +
                    $"{emotes.GetEmote(currency.ToString())} {bannerPrice} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), bannerPrice)}." +
                    $"\n\n{emotes.GetEmote("Arrow")} Найти приобретенный баннер можно в {emotes.GetEmote("DiscordSlashCommand")} `/баннеры`.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopBanner)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}