using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using static Emmy.Data.Enums.Discord.Role;

namespace Emmy.Services.Discord.Interactions.Attributes
{
    public class RequireRole : PreconditionAttribute
    {
        private readonly Data.Enums.Discord.Role _role;

        public RequireRole(Data.Enums.Discord.Role role)
        {
            _role = role;
        }

        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
            ICommandInfo commandInfo, IServiceProvider services)
        {
            var roles = DiscordRepository.Roles;
            var service = services.GetRequiredService<IMediator>();
            var hasRole = await service.Send(new CheckGuildUserHasRoleByTypeQuery(context.User.Id, _role));
            var hasAdminRole = await service.Send(new CheckGuildUserHasRoleByTypeQuery(context.User.Id, Administration));

            return hasRole
                ? PreconditionResult.FromSuccess()
                : hasAdminRole
                    ? PreconditionResult.FromSuccess()
                    : PreconditionResult.FromError(
                        $"это действие доступно лишь пользователям с ролью {roles[_role].Id.ToMention(MentionType.Role)}");
        }
    }
}