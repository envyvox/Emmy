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
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Key.Commands;
using Emmy.Services.Game.Key.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Shop
{
    [RequireLocation(Location.Neutral)]
    public class ShopKeyBuy : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopKeyBuy(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("shop-key-buy-currency:*")]
        public async Task ShopKeyBuyTask(string currencyHashcode, string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var currency = (Currency) int.Parse(currencyHashcode);
            var type = (KeyType) int.Parse(selectedValues.First());

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, currency));
            var key = await _mediator.Send(new GetKeyQuery(type));
            var keyPrice = currency is Currency.Token ? key.Price : key.Price.ConvertTokensToLobbs();

            if (userCurrency.Amount < keyPrice)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(currency.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), 5)} " +
                    $"для приобретения {emotes.GetEmote(type.EmoteName())} {_local.Localize(LocalizationCategory.Key, type.ToString(), 2)}.");
            }

            await _mediator.Send(new RemoveCurrencyFromUserCommand(user.Id, currency, keyPrice));
            await _mediator.Send(new AddKeyToUserCommand(user.Id, type));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин ключей")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно приобрел {emotes.GetEmote(type.EmoteName())} " +
                    $"{_local.Localize(LocalizationCategory.Key, type.ToString())} за " +
                    $"{emotes.GetEmote(currency.ToString())} {keyPrice} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), keyPrice)}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopKey)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}