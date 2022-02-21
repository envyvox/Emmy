using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Crop.Queries;
using Emmy.Services.Game.Fish.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.Shop
{
    [RequireLocation(Location.Neutral)]
    public class VendorPaginator : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public VendorPaginator(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("vendor-paginator:*,*")]
        public async Task Execute(string category, string pageString)
        {
            await Context.Interaction.DeferAsync(true);

            var page = int.Parse(pageString);
            int maxPage;

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Скупщик", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются товары которые скупщик готов купить:" +
                    $"\n{StringExtensions.EmptyChar}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Vendor)));

            switch (category)
            {
                case "рыба":
                {
                    var fishes = await _mediator.Send(new GetFishesQuery());
                    maxPage = (int) Math.Ceiling(fishes.Count / 10.0);

                    fishes = fishes
                        .Skip(page > 1 ? (page - 1) * 10 : 0)
                        .Take(10)
                        .ToList();

                    var counter = 0;
                    foreach (var fish in fishes)
                    {
                        counter++;

                        embed.AddField(
                            $"{emotes.GetEmote(fish.Name)} {_local.Localize(LocalizationCategory.Fish, fish.Name)}",
                            $"Стоимость: {emotes.GetEmote(Currency.Token.ToString())} {fish.Price} " +
                            $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), fish.Price)}",
                            true);

                        if (counter == 2)
                        {
                            counter = 0;
                            embed.AddEmptyField(true);
                        }
                    }

                    embed.WithFooter($"Страница {page} из {maxPage}");

                    break;
                }

                case "урожай":
                {
                    var crops = await _mediator.Send(new GetCropsQuery());
                    maxPage = (int) Math.Ceiling(crops.Count / 10.0);

                    crops = crops
                        .Skip(page > 1 ? (page - 1) * 10 : 0)
                        .Take(10)
                        .ToList();

                    var counter = 0;
                    foreach (var crop in crops)
                    {
                        counter++;

                        embed.AddField(
                            $"{emotes.GetEmote(crop.Name)} {_local.Localize(LocalizationCategory.Crop, crop.Name)}",
                            $"Стоимость: {emotes.GetEmote(Currency.Token.ToString())} {crop.Price} " +
                            $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), crop.Price)}",
                            true);

                        if (counter == 2)
                        {
                            counter = 0;
                            embed.AddEmptyField(true);
                        }
                    }

                    embed.WithFooter($"Страница {page} из {maxPage}");

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var components = new ComponentBuilder()
                .WithButton("Назад", $"vendor-paginator:{category},{page - 1}", disabled: page <= 1)
                .WithButton("Вперед", $"vendor-paginator:{category},{page + 1}", disabled: page >= maxPage)
                .WithButton(
                    @$"Продать {
                        category switch {
                            "рыба" => "всю рыбу",
                            "урожай" => "весь урожай",
                            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)}}",
                    $"vendor-sell:{category}",
                    ButtonStyle.Success);

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = components.Build();
            });
        }
    }
}