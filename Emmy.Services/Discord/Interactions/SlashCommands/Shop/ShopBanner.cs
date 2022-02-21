using System;
using System.Collections.Generic;
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
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.SlashCommands.Shop
{
    [RequireCommandChannel]
    [RequireLocation(Location.Neutral)]
    public class ShopBanner : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopBanner(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "магазин-баннеров",
            "Приобретай различные баннеры для своего профиля")]
        public async Task ShopBannerTask()
        {
            await Context.Interaction.DeferAsync(true);

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
                .Take(5)
                .ToList();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин баннеров", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются доступные для приобретения баннеры:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для приобретения баннера, **выбери его** из списка под этим сообщением." +
                    $"\n\n{emotes.GetEmote("Arrow")} Ты так же можешь заказать {emotes.GetEmote(BannerRarity.Custom.EmoteName())} " +
                    $"{BannerRarity.Custom.Localize().ToLower()} баннер, для этого необходимо написать в **личные сообщения** " +
                    $"{roles[Data.Enums.Discord.Role.Administration].Id.ToMention(MentionType.Role)}." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField(
                    $"Текущая валюта для оплаты {emotes.GetEmote("Arrow")} {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString())}",
                    StringExtensions.EmptyChar)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopBanner)))
                .WithFooter($"Страница 1 из {maxPages}");

            var components = new ComponentBuilder
            {
                ActionRows = new List<ActionRowBuilder>
                {
                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                                "Назад",
                                $"shop-banner-paginator:{Currency.Token.GetHashCode()},1",
                                isDisabled: true)
                            .Build())
                        .AddComponent(new ButtonBuilder(
                                "Вперед",
                                $"shop-banner-paginator:{Currency.Token.GetHashCode()},2")
                            .Build()),

                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                                "Оплата токенами",
                                $"shop-banner-select-currency:{Currency.Token.GetHashCode()},1",
                                ButtonStyle.Primary,
                                emote: Parse(emotes.GetEmote(Currency.Token.ToString())),
                                isDisabled: true)
                            .Build())
                        .AddComponent(new ButtonBuilder(
                                "Оплата лоббсами",
                                $"shop-banner-select-currency:{Currency.Lobbs.GetHashCode()},1",
                                ButtonStyle.Primary,
                                emote: Parse(emotes.GetEmote(Currency.Lobbs.ToString())))
                            .Build())
                }
            };

            var selectMenu = new SelectMenuBuilder()
                .WithCustomId($"shop-banner-buy-currency:{Currency.Token.GetHashCode()}")
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

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed,
                components
                    .WithSelectMenu(selectMenu)
                    .Build()));
        }
    }
}