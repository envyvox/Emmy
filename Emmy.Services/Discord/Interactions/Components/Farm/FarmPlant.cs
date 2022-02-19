using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Seed.Queries;
using Emmy.Services.Game.User.Queries;
using Humanizer;
using Humanizer.Localisation;
using MediatR;
using static Discord.Emote;
using static Emmy.Services.Extensions.ExceptionExtensions;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Farm
{
    [RequireLocation(Location.Neutral)]
    public class FarmPlant : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public FarmPlant(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("farm-plant:*")]
        public async Task Execute(string pageString)
        {
            await Context.Interaction.DeferAsync(true);

            var page = int.Parse(pageString);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userSeeds = await _mediator.Send(new GetUserSeedsQuery(user.Id));

            if (userSeeds.Count < 1)
            {
                throw new GameUserExpectedException(
                    $"у тебя нет семян которые можно было бы посадить на твою {emotes.GetEmote(Building.Farm.ToString())} ферму." +
                    $"\n\n{emotes.GetEmote("Arrow")} Приобрести семена можно в {emotes.GetEmote("DiscordSlashCommand")} `/магазин-семян`.");
            }

            var maxPages = (int) Math.Ceiling(userSeeds.Count / 5.0);

            userSeeds = userSeeds
                .Skip(page > 1 ? (page - 1) * 5 : 0)
                .Take(5)
                .ToList();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"для начала необходимо выбрать семена которые ты хочешь посадить на свою {emotes.GetEmote(Building.Farm.ToString())} ферму:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для посадки семян, **выбери их** из списка под этим сообщением." +
                    $"\n{StringExtensions.EmptyChar}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)))
                .WithFooter($"Страница {page} из {maxPages}");

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери семена которые хочешь посадить")
                .WithCustomId("farm-plant-selected-seed");

            foreach (var userSeed in userSeeds)
            {
                var seedDesc =
                    $"Через {userSeed.Seed.GrowthDays.Days().Humanize(1, new CultureInfo("ru-RU"), TimeUnit.Day)} вырастет " +
                    $"{emotes.GetEmote(userSeed.Seed.Crop.Name)} {_local.Localize(LocalizationCategory.Crop, userSeed.Seed.Crop.Name)} " +
                    $"стоимостью {emotes.GetEmote(Currency.Token.ToString())} {userSeed.Seed.Crop.Price} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), userSeed.Seed.Crop.Price)}";

                if (userSeed.Seed.IsMultiply)
                    seedDesc +=
                        $"\n{emotes.GetEmote("Arrow")} *Растет несколько шт. с одного семени*";

                if (userSeed.Seed.ReGrowthDays > 0)
                    seedDesc +=
                        $"\n{emotes.GetEmote("Arrow")} *После первого сбора будет давать урожай каждые " +
                        $"{userSeed.Seed.ReGrowthDays.Days().Humanize(1, new CultureInfo("ru-RU"), TimeUnit.Day)}*";

                embed.AddField(
                    $"{emotes.GetEmote(userSeed.Seed.Name)} {_local.Localize(LocalizationCategory.Seed, userSeed.Seed.Name)}, в наличии {userSeed.Amount} шт.",
                    seedDesc + $"\n{StringExtensions.EmptyChar}");

                selectMenu.AddOption(
                    _local.Localize(LocalizationCategory.Seed, userSeed.Seed.Name),
                    $"{userSeed.Seed.Id}",
                    emote: Parse(emotes.GetEmote(userSeed.Seed.Name)));
            }

            var components = new ComponentBuilder()
                .WithButton("Назад", $"farm-plant:{page - 1}", disabled: page <= 1)
                .WithButton("Вперед", $"farm-plant:{page + 1}", disabled: page >= maxPages)
                .WithSelectMenu(selectMenu);

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = components.Build();
            });
        }
    }
}