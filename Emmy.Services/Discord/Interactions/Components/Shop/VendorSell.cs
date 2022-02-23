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
using Emmy.Services.Game.Achievement.Commands;
using Emmy.Services.Game.Crop.Commands;
using Emmy.Services.Game.Crop.Queries;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Fish.Commands;
using Emmy.Services.Game.Fish.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Statistic.Commands;
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

            if (user.Fraction is Data.Enums.Fraction.Neutral)
            {
                throw new GameUserExpectedException(
                    "хоть это место и зовется **Нейтральной зоной**, местные не очень " +
                    $"доверяют {emotes.GetEmote(Data.Enums.Fraction.Neutral.EmoteName())} **нейтралам** и " +
                    "не собираются покупать рыбу у неизвестного рыбака." +
                    "\n\nТебе необходимо заручиться поддержкой фракции, ведь даже простое упоминание их имен открывает множество дверей." +
                    $"\n\n{emotes.GetEmote("Arrow")} Чтобы вступить во фракцию, напиши {emotes.GetEmote("DiscordSlashCommand")} `/фракция`.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Скупщик", Context.User.GetAvatarUrl())
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
                        await _mediator.Send(new AddStatisticToUserCommand(user.Id, Statistic.VendorSell, amount));

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
                        await _mediator.Send(new AddStatisticToUserCommand(user.Id, Statistic.VendorSell, amount));

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
            await _mediator.Send(new CheckAchievementsInUserCommand(user.Id, new[]
            {
                Achievement.FirstVendorDeal,
                Achievement.Vendor100Sell,
                Achievement.Vendor777Sell,
                Achievement.Vendor1500Sell,
                Achievement.Vendor3500Sell
            }));

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