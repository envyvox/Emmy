using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Container.Commands;
using Emmy.Services.Game.Container.Queries;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components
{
    public class ContainerOpen : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        private readonly Random _random = new();

        public ContainerOpen(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("container-open:*")]
        public async Task Execute(string containerHashcodeString)
        {
            await Context.Interaction.DeferAsync();

            var container = (Container) int.Parse(containerHashcodeString);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userContainer = await _mediator.Send(new GetUserContainerQuery(user.Id, container));

            if (userContainer.Amount < 1)
            {
                throw new GameUserExpectedException(
                    $"у тебя нет в наличии ни одного {emotes.GetEmote(container.EmoteName())} " +
                    $"{_local.Localize(LocalizationCategory.Container, container.ToString(), 2)} чтобы открыть.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Открытие контейнеров");

            switch (container)
            {
                case Container.Token:
                {
                    var minAmount = (int) await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.ContainerTokenMinAmount));
                    var maxAmount = (int) await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.ContainerTokenMaxAmount));

                    var receivedAmount = 0;
                    for (var i = 0; i < userContainer.Amount; i++)
                    {
                        receivedAmount += _random.Next(minAmount, maxAmount + 1);
                    }

                    await _mediator.Send(new RemoveContainerFromUserCommand(user.Id, container, userContainer.Amount));
                    await _mediator.Send(new AddCurrencyToUserCommand(user.Id, Currency.Token, (uint) receivedAmount));

                    embed.WithDescription(
                        $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                        $"ты открываешь {emotes.GetEmote(container.EmoteName())} {userContainer.Amount} " +
                        $"{_local.Localize(LocalizationCategory.Container, container.ToString(), userContainer.Amount)} " +
                        $"и обнаруживаешь внутри {emotes.GetEmote(Currency.Token.ToString())} {receivedAmount} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), (uint) receivedAmount)}.");

                    break;
                }

                case Container.Supply:
                {
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}