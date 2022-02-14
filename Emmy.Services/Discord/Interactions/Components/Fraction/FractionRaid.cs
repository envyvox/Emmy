using System.Threading.Tasks;
using Discord.Interactions;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.Fraction
{
    public class FractionRaid : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public FractionRaid(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("fraction-raid")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();
        }
    }
}