using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Emote.Models;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Container.Models;
using Emmy.Services.Game.Container.Queries;
using Emmy.Services.Game.Crop.Models;
using Emmy.Services.Game.Crop.Queries;
using Emmy.Services.Game.Currency.Models;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Fish.Models;
using Emmy.Services.Game.Fish.Queries;
using Emmy.Services.Game.Key.Models;
using Emmy.Services.Game.Key.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Seed.Models;
using Emmy.Services.Game.Seed.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    public class UserInventory : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;
        private Dictionary<string, EmoteDto> _emotes;

        public UserInventory(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "инвентарь",
            "Просмотр игрового инвентаря")]
        public async Task UserInventoryTask(
            [Summary("категория", "Категория предметов которые ты хочешь посмотреть")]
            [Choice("Рыба", "рыба")]
            [Choice("Семена", "семена")]
            [Choice("Урожай", "урожай")]
            string category = null)
        {
            await Context.Interaction.DeferAsync(true);

            _emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Инвентарь")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserInventory)));

            var desc = string.Empty;
            if (category is null)
            {
                desc = "все полученные предметы попадают сюда:";
                var userCurrencies = await _mediator.Send(new GetUserCurrenciesQuery(user.Id));
                var userContainers = await _mediator.Send(new GetUserContainersQuery(user.Id));
                var userKeys = await _mediator.Send(new GetUserKeysQuery(user.Id));
                var userFishes = await _mediator.Send(new GetUserFishesQuery(user.Id));
                var userSeeds = await _mediator.Send(new GetUserSeedsQuery(user.Id));
                var userCrops = await _mediator.Send(new GetUserCropsQuery(user.Id));

                embed
                    .AddField("Валюта", DisplayUserCurrencies(userCurrencies))
                    .AddField("Контейнеры", DisplayUserContainers(userContainers));

                if (userKeys.Any())
                {
                    embed.AddField("Ключи", DisplayUserKeys(userKeys));
                }

                if (userFishes.Any())
                {
                    embed.AddField("Рыба", DisplayUserFishes(userFishes));
                }

                if (userSeeds.Any())
                {
                    embed.AddField("Семена", DisplayUserSeeds(userSeeds));
                }

                if (userCrops.Any())
                {
                    embed.AddField("Урожай", DisplayUserCrops(userCrops));
                }
            }
            else
            {
                switch (category)
                {
                    case "рыба":
                    {
                        desc = "тут отображается твоя рыба:";
                        var userFishes = await _mediator.Send(new GetUserFishesQuery(user.Id));

                        foreach (var rarity in Enum.GetValues(typeof(FishRarity)).Cast<FishRarity>())
                        {
                            embed.AddField(rarity.Localize(),
                                DisplayUserFishes(userFishes.Where(x => x.Fish.Rarity == rarity)));
                        }

                        break;
                    }
                    case "семена":

                        desc = "тут отображается твои семена:";
                        var userSeeds = await _mediator.Send(new GetUserSeedsQuery(user.Id));

                        foreach (var season in Enum
                            .GetValues(typeof(Season))
                            .Cast<Season>()
                            .Where(x => x != Season.Any))
                        {
                            embed.AddField(season.Localize(),
                                DisplayUserSeeds(userSeeds.Where(x => x.Seed.Season == season)));
                        }

                        break;
                    case "урожай":

                        desc = "тут отображается твой урожай:";
                        var userCrops = await _mediator.Send(new GetUserCropsQuery(user.Id));

                        foreach (var season in Enum
                            .GetValues(typeof(Season))
                            .Cast<Season>()
                            .Where(x => x != Season.Any))
                        {
                            embed.AddField(season.Localize(),
                                DisplayUserCrops(userCrops.Where(x => x.Crop.Seed.Season == season)));
                        }

                        break;
                }
            }

            embed.WithDescription(
                $"{Context.User.Mention.AsGameMention(user.Title)}, " + desc + $"\n{StringExtensions.EmptyChar}");

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }

        private string DisplayUserCurrencies(IReadOnlyDictionary<Currency, UserCurrencyDto> userCurrencies)
        {
            var str = Enum
                .GetValues(typeof(Currency))
                .Cast<Currency>()
                .Aggregate(string.Empty, (s, v) =>
                    s +
                    $"{_emotes.GetEmote(v.ToString())} {(userCurrencies.ContainsKey(v) ? userCurrencies[v].Amount : 0)} " +
                    $"{_local.Localize(LocalizationCategory.Currency, v.ToString(), userCurrencies.ContainsKey(v) ? userCurrencies[v].Amount : 0)}, ");

            return str.RemoveFromEnd(2);
        }

        private string DisplayUserContainers(IReadOnlyDictionary<Container, UserContainerDto> userContainers)
        {
            var str = Enum
                .GetValues(typeof(Container))
                .Cast<Container>()
                .Aggregate(string.Empty, (s, v) =>
                    s +
                    $"{_emotes.GetEmote(v.EmoteName())} {(userContainers.ContainsKey(v) ? userContainers[v].Amount : 0)} " +
                    $"{_local.Localize(LocalizationCategory.Container, v.ToString(), userContainers.ContainsKey(v) ? userContainers[v].Amount : 0)}, ");

            return str.RemoveFromEnd(2);
        }

        private string DisplayUserKeys(IEnumerable<UserKeyDto> userKeys)
        {
            var str = userKeys.Aggregate(string.Empty, (s, v) =>
                s +
                $"{_emotes.GetEmote(v.Type.EmoteName())} {v.Amount} {_local.Localize(LocalizationCategory.Key, v.Type.ToString(), v.Amount)}, ");

            return str.RemoveFromEnd(2);
        }

        private string DisplayUserFishes(IEnumerable<UserFishDto> userFishes)
        {
            var str = userFishes.Aggregate(string.Empty, (s, v) =>
                s +
                $"{_emotes.GetEmote(v.Fish.Name)} {v.Amount} {_local.Localize(LocalizationCategory.Fish, v.Fish.Name, v.Amount)}, ");

            return str.Length > 0
                ? str.Length > 1024
                    ? "У тебя слишком много рыбы, напиши `/инвентарь рыба` чтобы посмотреть ее"
                    : str.RemoveFromEnd(2)
                : "У тебя нет ни одного предмета этого типа";
        }

        private string DisplayUserSeeds(IEnumerable<UserSeedDto> userSeeds)
        {
            var str = userSeeds.Aggregate(string.Empty, (s, v) =>
                s +
                $"{_emotes.GetEmote(v.Seed.Name)} {v.Amount} {_local.Localize(LocalizationCategory.Seed, v.Seed.Name, v.Amount)}, ");

            return str.Length > 0
                ? str.Length > 1024
                    ? "У тебя слишком много рыбы, напиши `/инвентарь семена` чтобы посмотреть ее"
                    : str.RemoveFromEnd(2)
                : "У тебя нет ни одного предмета этого типа";
        }

        private string DisplayUserCrops(IEnumerable<UserCropDto> userCrops)
        {
            var str = userCrops.Aggregate(string.Empty, (s, v) =>
                s +
                $"{_emotes.GetEmote(v.Crop.Name)} {v.Amount} {_local.Localize(LocalizationCategory.Crop, v.Crop.Name, v.Amount)}, ");

            return str.Length > 0
                ? str.Length > 1024
                    ? "У тебя слишком много рыбы, напиши `/инвентарь урожай` чтобы посмотреть ее"
                    : str.RemoveFromEnd(2)
                : "У тебя нет ни одного предмета этого типа";
        }
    }
}