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
    public class UserRolesActivate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserRolesActivate(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("user-roles-activate")]
        public async Task UserRolesActivateTask(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync(true);

            var selectedRolesIds = selectedValues.Select(ulong.Parse);

            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var addedRolesString = string.Empty;
            foreach (var roleId in selectedRolesIds)
            {
                var role = await _mediator.Send(new GetSocketRoleQuery(roleId));
                var hasRole = await _mediator.Send(new CheckGuildUserHasRoleByIdQuery(Context.User.Id, role.Id));

                if (hasRole) continue;

                await _mediator.Send(new AddRoleToGuildUserByRoleIdCommand(Context.User.Id, role.Id));

                addedRolesString += $"{role.Mention}, ";
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Роли")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    (addedRolesString.Length > 0
                        ? $"ты успешно надел следующие роли: {addedRolesString.RemoveFromEnd(2)}."
                        : "манипуляция ролями ничего не изменила."))
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserRoles)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}