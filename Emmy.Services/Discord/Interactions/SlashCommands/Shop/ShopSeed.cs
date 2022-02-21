using System;
using System.Globalization;
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
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Seed.Queries;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using Humanizer.Localisation;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.SlashCommands.Shop
{
    [RequireCommandChannel]
    [RequireLocation(Location.Neutral)]
    public class ShopSeed : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopSeed(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "магазин-семян",
            "Приобретай различные сезонные семена для выращивания урожая")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var currentSeason = await _mediator.Send(new GetCurrentSeasonQuery());
            var seeds = await _mediator.Send(new GetSeedsBySeasonQuery(currentSeason));

            var maxPage = (int) Math.Ceiling(seeds.Count / 5.0);

            var components = new ComponentBuilder()
                .WithButton("Назад", "shop-seed-paginator:1", disabled: true)
                .WithButton("Вперед", "shop-seed-paginator:2", disabled: seeds.Count <= 5);

            seeds = seeds
                .Take(5)
                .ToList();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин семян", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут можно приобрести различные сезонные семена для выращивания урожая:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для прибретения семян, **выбери их** из меню под этим сообщением." +
                    $"\n\n{emotes.GetEmote("Arrow")} Это динамический магазин, товары которого обновляются каждый " +
                    "сезон, не пропускай!" +
                    $"\n{Services.Extensions.StringExtensions.EmptyChar}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopSeed)))
                .WithFooter($"Страница 1 из {maxPage}");

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери семена которые хочешь приобрести")
                .WithCustomId("shop-seed-buy");

            foreach (var seed in seeds)
            {
                var seedDesc =
                    $"Через {seed.GrowthDays.Days().Humanize(1, new CultureInfo("ru-RU"), TimeUnit.Day)} вырастет " +
                    $"{emotes.GetEmote(seed.Crop.Name)} {_local.Localize(LocalizationCategory.Crop, seed.Crop.Name)} " +
                    $"стоимостью {emotes.GetEmote(Currency.Token.ToString())} {seed.Crop.Price} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), seed.Crop.Price)}";

                if (seed.IsMultiply)
                    seedDesc +=
                        $"\n{emotes.GetEmote("Arrow")} *Растет несколько шт. с одного семени*";

                if (seed.ReGrowthDays > 0)
                    seedDesc +=
                        $"\n{emotes.GetEmote("Arrow")} *После первого сбора будет давать урожай каждые " +
                        $"{seed.ReGrowthDays.Days().Humanize(1, new CultureInfo("ru-RU"), TimeUnit.Day)}*";

                embed.AddField(
                    $"{emotes.GetEmote(seed.Name)} 5 {_local.Localize(LocalizationCategory.Seed, seed.Name, 5)} стоимостью " +
                    $"{emotes.GetEmote(Currency.Token.ToString())} {seed.Price * 5} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), seed.Price * 5)}",
                    seedDesc + $"\n{Services.Extensions.StringExtensions.EmptyChar}");

                selectMenu.AddOption(
                    $"5 {_local.Localize(LocalizationCategory.Seed, seed.Name, 5)}",
                    $"{seed.Id}",
                    emote: Parse(emotes.GetEmote(seed.Name)));
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed,
                components.WithSelectMenu(selectMenu).Build()));
        }
    }
}