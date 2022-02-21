using System.Globalization;
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
    public class UserProfileUpdateCommandColorModal : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserProfileUpdateCommandColorModal(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ModalInteraction("user-profile-update-commandcolor-modal")]
        public async Task Execute(UpdateCommandColorModal modal)
        {
            await Context.Interaction.DeferAsync();

            var newColor = modal.CommandColor.Replace("#", "");

            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            await _mediator.Send(new UpdateUserCommandColorCommand(user.Id, newColor));

            var embed = new EmbedBuilder()
                .WithColor(new Color(uint.Parse(newColor, NumberStyles.HexNumber)))
                .WithAuthor("Изменение цвета команд", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "цвет команд успешно изменен.");

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}