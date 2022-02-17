using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.Components.Shop
{
    [RequireLocation(Location.Neutral)]
    public class ShopBannerSwitchCurrency : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopBannerSwitchCurrency(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("shop-banner-select-currency:*,*")]
        public async Task ShopBannerSwitchCurrencyTask(string currencyHashcode, string pageString)
        {
            await Context.Interaction.DeferAsync(true);

            var currency = (Currency) int.Parse(currencyHashcode);
            var page = int.Parse(pageString);

            var emotes = DiscordRepository.Emotes;
            var roles = DiscordRepository.Roles;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var banners = await _mediator.Send(new GetBannersQuery());

            banners = banners
                .Where(x =>
                    x.Rarity is BannerRarity.Common
                        or BannerRarity.Rare
                        or BannerRarity.Animated)
                .ToList();

            var maxPages = (int) Math.Ceiling(banners.Count / 5.0);

            banners = banners
                .Skip(page > 1 ? (page - 1) * 5 : 0)
                .Take(5)
                .ToList();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин баннеров")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются доступные для приобретения баннеры:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для приобретения баннера, **выбери его** из списка под этим сообщением." +
                    $"\n\n{emotes.GetEmote("Arrow")} Ты так же можешь заказать {emotes.GetEmote(BannerRarity.Custom.EmoteName())} " +
                    $"{BannerRarity.Custom.Localize().ToLower()} баннер, для этого необходимо написать в **личные сообщения** " +
                    $"{roles[Data.Enums.Discord.Role.Administration].Id.ToMention(MentionType.Role)}." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField(
                    $"Текущая валюта для оплаты {emotes.GetEmote("Arrow")} {emotes.GetEmote(currency.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString())}",
                    StringExtensions.EmptyChar)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopBanner)))
                .WithFooter($"Страница {page} из {maxPages}");

            var components = new ComponentBuilder
            {
                ActionRows = new List<ActionRowBuilder>
                {
                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                                "Назад",
                                $"shop-banner-paginator:{currencyHashcode},{page - 1}",
                                isDisabled: page <= 1)
                            .Build())
                        .AddComponent(new ButtonBuilder(
                                "Вперед",
                                $"shop-banner-paginator:{currencyHashcode},{page + 1}",
                                isDisabled: page >= maxPages)
                            .Build()),

                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                                "Оплата токенами",
                                $"shop-banner-select-currency:{Currency.Token.GetHashCode()},{pageString}",
                                ButtonStyle.Primary,
                                emote: Parse(emotes.GetEmote(Currency.Token.ToString())),
                                isDisabled: currency is Currency.Token)
                            .Build())
                        .AddComponent(new ButtonBuilder(
                                "Оплата лоббсами",
                                $"shop-banner-select-currency:{Currency.Lobbs.GetHashCode()},{pageString}",
                                ButtonStyle.Primary,
                                emote: Parse(emotes.GetEmote(Currency.Lobbs.ToString())),
                                isDisabled: currency is Currency.Lobbs)
                            .Build())
                }
            };

            var selectMenu = new SelectMenuBuilder()
                .WithCustomId($"shop-banner-buy-currency:{currencyHashcode}")
                .WithPlaceholder("Выбери баннер который хочешь приобрести");

            foreach (var banner in banners)
            {
                embed.AddField(
                    $"{emotes.GetEmote(banner.Rarity.EmoteName())} {banner.Rarity.Localize()} «{banner.Name}»",
                    $"[Нажми сюда чтобы посмотреть]({banner.Url})" +
                    $"\nСтоимость: {emotes.GetEmote(Currency.Token.ToString())} {banner.Price} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), banner.Price)} или " +
                    $"{emotes.GetEmote(Currency.Lobbs.ToString())} {banner.Price.ConvertTokensToLobbs()} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), banner.Price.ConvertTokensToLobbs())}");

                selectMenu.AddOption(banner.Name.ToLower(), $"{banner.Id}",
                    emote: Parse(emotes.GetEmote(banner.Rarity.EmoteName())));
            }

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = components.WithSelectMenu(selectMenu).Build();
            });
        }
    }
}