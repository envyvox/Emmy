using System.Threading.Tasks;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Interactions.Attributes;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.Fraction
{
    [RequireLocation(Location.Neutral)]
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