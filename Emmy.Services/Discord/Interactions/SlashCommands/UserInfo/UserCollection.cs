using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Collection.Queries;
using Emmy.Services.Game.Crop.Queries;
using Emmy.Services.Game.Fish.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    public class UserCollection : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public UserCollection(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "коллекция",
            "Просмотр своей коллекции")]
        public async Task Execute(
            [Summary("категория", "Категория коллекции которую ты хочешь посмотреть")]
            [Choice("Урожай", 1)]
            [Choice("Рыба", 2)]
            int collectionCategoryHashcode)
        {
            await Context.Interaction.DeferAsync(true);

            var category = (CollectionCategory) collectionCategoryHashcode;

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCollections = await _mediator.Send(new GetUserCollectionsQuery(user.Id, category));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Коллекция")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"тут отображается твоя коллекция в категории **{category.Localize()}**:" +
                    $"\n{StringExtensions.EmptyChar}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserCollection)));

            switch (category)
            {
                case CollectionCategory.Crop:

                    var crops = await _mediator.Send(new GetCropsQuery());
                    var springCropString = string.Empty;
                    var summerCropString = string.Empty;
                    var autumnCropString = string.Empty;

                    foreach (var crop in crops)
                    {
                        var exist = userCollections.Any(x => x.ItemId == crop.Id);
                        var displayString =
                            $"{emotes.GetEmote(crop.Name + (exist ? "" : "BW"))} " +
                            $"{_local.Localize(LocalizationCategory.Crop, crop.Name)} ";

                        switch (crop.Seed.Season)
                        {
                            case Season.Spring:
                                springCropString += displayString;
                                break;
                            case Season.Summer:
                                summerCropString += displayString;
                                break;
                            case Season.Autumn:
                                autumnCropString += displayString;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    embed
                        .AddField("Весенний урожай", springCropString)
                        .AddField("Летний урожай", summerCropString)
                        .AddField("Осенний урожай", autumnCropString);

                    break;
                case CollectionCategory.Fish:

                    var fishes = await _mediator.Send(new GetFishesQuery());
                    var commonFishString = string.Empty;
                    var rareFishString = string.Empty;
                    var epicFishString = string.Empty;
                    var mythicalFishString = string.Empty;
                    var legendaryFishString = string.Empty;

                    foreach (var fish in fishes)
                    {
                        var exist = userCollections.Any(x => x.ItemId == fish.Id);
                        var displayString =
                            $"{emotes.GetEmote(fish.Name + (exist ? "" : "BW"))} " +
                            $"{_local.Localize(LocalizationCategory.Fish, fish.Name)} ";

                        switch (fish.Rarity)
                        {
                            case FishRarity.Common:
                                commonFishString += displayString;
                                break;
                            case FishRarity.Rare:
                                rareFishString += displayString;
                                break;
                            case FishRarity.Epic:
                                epicFishString += displayString;
                                break;
                            case FishRarity.Mythical:
                                mythicalFishString += displayString;
                                break;
                            case FishRarity.Legendary:
                                legendaryFishString += displayString;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    embed
                        .AddField(FishRarity.Common.Localize(), commonFishString)
                        .AddField(FishRarity.Rare.Localize(), rareFishString)
                        .AddField(FishRarity.Epic.Localize(), epicFishString)
                        .AddField(FishRarity.Mythical.Localize(), mythicalFishString)
                        .AddField(FishRarity.Legendary.Localize(), legendaryFishString);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}