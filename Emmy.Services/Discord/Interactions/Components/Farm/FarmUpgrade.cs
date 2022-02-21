using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
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
    [RequireLocation(Location.Neutral)]
    public class FarmUpgrade : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public FarmUpgrade(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("farm-upgrade:*")]
        public async Task Execute(string buildingHashcode)
        {
            await Context.Interaction.DeferAsync(true);

            var building = (Building) int.Parse(buildingHashcode);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasBuilding = await _mediator.Send(new CheckUserHasBuildingQuery(user.Id, building));

            if (hasBuilding)
            {
                throw new GameUserExpectedException(
                    $"ты уже приобретал {emotes.GetEmote(building.ToString())} это улучшение участка.");
            }

            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, Currency.Token));
            var buildingPrice = await _mediator.Send(new GetWorldPropertyValueQuery(building switch
            {
                Building.FarmExpansionL1 => WorldProperty.FarmExpansionL1Price,
                Building.FarmExpansionL2 => WorldProperty.FarmExpansionL2Price,
                _ => throw new ArgumentOutOfRangeException()
            }));

            if (userCurrency.Amount < buildingPrice)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), 5)} " +
                    $"для приобретения {emotes.GetEmote(building.ToString())} расширения участка.");
            }

            await _mediator.Send(new RemoveCurrencyFromUserCommand(user.Id, Currency.Token, buildingPrice));
            await _mediator.Send(new CreateUserBuildingCommand(user.Id, building));
            await _mediator.Send(new CreateUserFarmsCommand(user.Id, building switch
            {
                Building.FarmExpansionL1 => new uint[] {6, 7},
                Building.FarmExpansionL2 => new uint[] {8, 9, 10},
                _ => throw new ArgumentOutOfRangeException()
            }));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно приобрел {emotes.GetEmote(building.ToString())} улучшение для своей " +
                    $"{emotes.GetEmote(Building.Farm.ToString())} фермы за {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{buildingPrice} {_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), buildingPrice)}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().Build();
            });
        }
    }
}