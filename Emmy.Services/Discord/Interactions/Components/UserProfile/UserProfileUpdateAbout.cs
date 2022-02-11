using System.Threading.Tasks;
using Discord.Interactions;

namespace Emmy.Services.Discord.Interactions.Components.UserProfile
{
    public class UserProfileUpdateAbout : InteractionModuleBase<SocketInteractionContext>
    {
        [ComponentInteraction("user-profile-update-about")]
        public async Task Execute()
        {
            await Context.Interaction.RespondWithModalAsync<UpdateAboutModal>("user-profile-update-about-modal");
        }
    }
}