using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.UserRoles
{
    public class UserRolesDeactivate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserRolesDeactivate(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("user-roles-deactivate")]
        public async Task UserRolesDeactivateTask(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync(true);

            var selectedRolesIds = selectedValues.Select(ulong.Parse);

            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var removedRolesString = string.Empty;
            foreach (var roleId in selectedRolesIds)
            {
                var role = await _mediator.Send(new GetSocketRoleQuery(roleId));
                var hasRole = await _mediator.Send(new CheckGuildUserHasRoleByIdQuery(Context.User.Id, role.Id));

                if (hasRole is false) continue;

                await _mediator.Send(new RemoveRoleFromGuildUserByRoleIdCommand(Context.User.Id, role.Id));

                removedRolesString += $"{role.Mention}, ";
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Роли")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    (removedRolesString.Length > 0
                        ? $"ты успешно снял следующие роли: {removedRolesString.RemoveFromEnd(2)}."
                        : "манипуляция ролями ничего не изменила."))
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserRoles)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}