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
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Seed.Commands;
using Emmy.Services.Game.Seed.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Shop
{
    [RequireLocation(Location.Neutral)]
    public class ShopSeedBuy : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopSeedBuy(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("shop-seed-buy")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var seedId = Guid.Parse(selectedValues.First());

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var seed = await _mediator.Send(new GetSeedQuery(seedId));
            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, Currency.Token));

            if (userCurrency.Amount < seed.Price * 5)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), 5)} " +
                    $"для приобретения {emotes.GetEmote(seed.Name)} 5 " +
                    $"{_local.Localize(LocalizationCategory.Seed, seed.Name, 5)}.");
            }

            await _mediator.Send(new RemoveCurrencyFromUserCommand(user.Id, Currency.Token, seed.Price * 5));
            await _mediator.Send(new AddSeedToUserCommand(user.Id, seed.Id, 5));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин семян")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно приобрел {emotes.GetEmote(seed.Name)} 5 " +
                    $"{_local.Localize(LocalizationCategory.Seed, seed.Name, 5)} за " +
                    $"{emotes.GetEmote(Currency.Token.ToString())} {seed.Price * 5} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), seed.Price * 5)}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopSeed)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}