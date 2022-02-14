using System.Threading.Tasks;
using Discord.Interactions;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.Fraction
{
    public class FractionQuestsWeekly : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public FractionQuestsWeekly(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("fraction-quests-weekly")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();
        }
    }
}