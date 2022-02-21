using System.Threading.Tasks;
using Discord.Interactions;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.UserProfile
{
    public class UserProfileUpdateCommandColor : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserProfileUpdateCommandColor(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("user-profile-update-commandcolor")]
        public async Task Execute()
        {
            await Context.Interaction.RespondWithModalAsync<UpdateCommandColorModal>(
                "user-profile-update-commandcolor-modal");
        }
    }
}