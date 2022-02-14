using System.Threading.Tasks;
using Discord.Interactions;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.Fraction
{
    public class FractionQuestsDaily : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public FractionQuestsDaily(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("fraction-quests-daily")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();
        }
    }
}