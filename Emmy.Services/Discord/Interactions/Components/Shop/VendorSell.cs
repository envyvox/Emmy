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
using Emmy.Services.Game.Crop.Commands;
using Emmy.Services.Game.Crop.Queries;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Fish.Commands;
using Emmy.Services.Game.Fish.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Shop
{
    [RequireLocation(Location.Neutral)]
    public class VendorSell : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public VendorSell(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("vendor-sell:*")]
        public async Task Execute(string category)
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Скупщик")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Vendor)));

            uint totalCurrencyAmount = 0;
            var soldItems = string.Empty;

            switch (category)
            {
                case "рыба":
                {
                    var userFishes = await _mediator.Send(new GetUserFishesQuery(user.Id));

                    if (userFishes.Any() is false)
                    {
                        throw new GameUserExpectedException(
                            "у тебя нет ни одной рыбы которую можно было бы продать скупщику.");
                    }

                    foreach (var (fish, amount) in userFishes)
                    {
                        await _mediator.Send(new RemoveFishFromUserCommand(user.Id, fish.Id, amount));

                        var currencyAmount = fish.Price * amount;
                        totalCurrencyAmount += currencyAmount;

                        soldItems +=
                            $"{emotes.GetEmote(fish.Name)} {amount} " +
                            $"{_local.Localize(LocalizationCategory.Fish, fish.Name, amount)} " +
                            $"за {emotes.GetEmote(Currency.Token.ToString())} {currencyAmount} " +
                            $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), currencyAmount)}\n";
                    }

                    break;
                }

                case "урожай":
                {
                    var userCrops = await _mediator.Send(new GetUserCropsQuery(user.Id));

                    if (userCrops.Any() is false)
                    {
                        throw new GameUserExpectedException(
                            "у тебя нет ни одного урожая который можно было бы продать скупщику");
                    }

                    foreach (var (crop, amount) in userCrops)
                    {
                        await _mediator.Send(new RemoveCropFromUserCommand(user.Id, crop.Id, amount));

                        var currencyAmount = crop.Price * amount;
                        totalCurrencyAmount += currencyAmount;

                        soldItems +=
                            $"{emotes.GetEmote(crop.Name)} {amount} " +
                            $"{_local.Localize(LocalizationCategory.Crop, crop.Name, amount)} " +
                            $"за {emotes.GetEmote(Currency.Token.ToString())} {currencyAmount} " +
                            $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), currencyAmount)}\n";
                    }

                    break;
                }
            }

            await _mediator.Send(new AddCurrencyToUserCommand(user.Id, Currency.Token, totalCurrencyAmount));

            var descString =
                soldItems +
                $"\n\nИтоговая прибыль {emotes.GetEmote(Currency.Token.ToString())} {totalCurrencyAmount} " +
                $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), totalCurrencyAmount)}";

            embed
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "после достаточно быстрого пересчета товаров со скупщиком, вот что получилось в итоге:" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Отчетность о продаже",
                    descString.Length > 1024
                        ? "Отчестность была такой длинной, что ты решил сразу взглянуть на самое важное" +
                          $"\n\nИтоговая прибыль {emotes.GetEmote(Currency.Token.ToString())} {totalCurrencyAmount} " +
                          $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), totalCurrencyAmount)}"
                        : descString);

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}