using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.UserProfile
{
    public class UserProfileUpdateAboutModal : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserProfileUpdateAboutModal(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ModalInteraction("user-profile-update-about-modal")]
        public async Task Execute(UpdateAboutModal modal)
        {
            await Context.Interaction.DeferAsync();

            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            await _mediator.Send(new UpdateUserAboutCommand(user.Id, modal.About));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Профиль")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "информация в твоем профиле была успешно обновлена.");

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}