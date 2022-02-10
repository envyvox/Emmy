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
using Emmy.Services.Game.Crop.Queries;
using Emmy.Services.Game.Fish.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    [RequireLocation(Location.Neutral)]
    public class Vendor : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public Vendor(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "скупщик",
            "Продавай свои предметы по отличной цене")]
        public async Task Execute(
            [Choice("рыба", "рыба")]
            [Choice("урожай", "урожай")]
            [Summary("категория", "Категория предмета который ты хочешь продать")]
            string category)
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Скупщик")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются товары которые скупщик готов купить:" +
                    $"\n{StringExtensions.EmptyChar}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Vendor)));

            var components = new ComponentBuilder()
                .WithButton("Назад", $"vendor-paginator:{category},1", disabled: true)
                .WithButton("Вперед", $"vendor-paginator:{category},2")
                .WithButton(
                    @$"Продать {
                        category switch {
                            "рыба" => "всю рыбу",
                            "урожай" => "весь урожай",
                            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)}}",
                    $"vendor-sell:{category}",
                    ButtonStyle.Success);

            switch (category)
            {
                case "рыба":
                {
                    var fishes = await _mediator.Send(new GetFishesQuery());
                    var maxPage = (int) Math.Ceiling(fishes.Count / 10.0);

                    fishes = fishes
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

                    embed.WithFooter($"Страница 1 из {maxPage}");

                    break;
                }

                case "урожай":
                {
                    var crops = await _mediator.Send(new GetCropsQuery());
                    var maxPage = (int) Math.Ceiling(crops.Count / 10.0);

                    crops = crops
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

                    embed.WithFooter($"Страница 1 из {maxPage}");

                    break;
                }
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build()));
        }
    }
}