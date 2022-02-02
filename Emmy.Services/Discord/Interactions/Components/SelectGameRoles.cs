using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components
{
    public class SelectGameRoles : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public SelectGameRoles(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("select-game-roles")]
        public async Task SelectGameRolesTask(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync(true);

            var selectedRoles = selectedValues
                .Select(x => (Data.Enums.Discord.Role) int.Parse(x))
                .ToArray();

            var roles = DiscordRepository.Roles;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var guildUser = await _mediator.Send(new GetSocketGuildUserQuery(Context.User.Id));

            var userRoles = guildUser.Roles
                .Where(socketRole => RoleHelper.GameRoles()
                    .Select(role => role.Name())
                    .Contains(socketRole.Name))
                .ToArray();

            var rolesToRemove = userRoles
                .Where(socketRole => selectedRoles
                    .Select(role => role.Name())
                    .Contains(socketRole.Name) is false);

            var rolesToAdd = selectedRoles
                .Where(role => userRoles
                    .All(socketRole => socketRole.Name != role.Name()));

            var addedRoles = string.Empty;
            var removedRoles = string.Empty;

            foreach (var role in rolesToRemove)
            {
                await _mediator.Send(new RemoveRoleFromGuildUserByRoleIdCommand(guildUser.Id, role.Id));

                removedRoles += $"{role.Id.ToMention(MentionType.Role)}, ";
            }

            foreach (var role in rolesToAdd)
            {
                await _mediator.Send(new AddRoleToGuildUserByRoleTypeCommand(guildUser.Id, role));

                addedRoles += $"{roles[role].Id.ToMention(MentionType.Role)}, ";
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithDescription(
                    (addedRoles.Length > 0
                        ? $"Ты успешно получил роли: {addedRoles.RemoveFromEnd(2)}\n"
                        : "") +
                    (removedRoles.Length > 0
                        ? $"Ты успешно снял роли: {removedRoles.RemoveFromEnd(2)}"
                        : ""));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));
        }
    }
}