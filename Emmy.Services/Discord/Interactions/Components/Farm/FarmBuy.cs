using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Building.Commands;
using Emmy.Services.Game.Building.Queries;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Farm.Commands;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Farm
{
    public class FarmBuy : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public FarmBuy(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("farm-buy")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasFarm = await _mediator.Send(new CheckUserHasBuildingQuery(user.Id, Building.Farm));

            if (hasFarm)
            {
                throw new GameUserExpectedException(
                    $"у тебя уже есть {emotes.GetEmote(Building.Farm.ToString())} ферма.");
            }

            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, Currency.Token));
            var farmPrice = await _mediator.Send(new GetWorldPropertyValueQuery(WorldProperty.FarmPrice));

            if (userCurrency.Amount < farmPrice)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), 5)} " +
                    $"для приобретения {emotes.GetEmote(Building.Farm.ToString())} фермы.");
            }

            await _mediator.Send(new RemoveCurrencyFromUserCommand(user.Id, Currency.Token, farmPrice));
            await _mediator.Send(new CreateUserBuildingCommand(user.Id, Building.Farm));
            await _mediator.Send(new CreateUserFarmsCommand(user.Id, new uint[] {1, 2, 3, 4, 5}));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно приобрел {emotes.GetEmote(Building.Farm.ToString())} ферму за " +
                    $"{emotes.GetEmote(Currency.Token.ToString())} {farmPrice} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), farmPrice)}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}