using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Game.World.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Emmy.Services.Discord.Interactions.Attributes
{
    public class RequireModuleEnabled : PreconditionAttribute
    {
        private readonly CommandModule _module;

        public RequireModuleEnabled(CommandModule module)
        {
            _module = module;
        }

        public override async Task<PreconditionResult> CheckRequirementsAsync(
            IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            var service = services.GetRequiredService<IMediator>();
            var isEnabled = await service.Send(new CheckModuleEnabledQuery(_module));
            var hasAdminRole = await service.Send(new CheckGuildUserHasRoleByTypeQuery(
                context.User.Id, Data.Enums.Discord.Role.Administration));

            return isEnabled
                ? PreconditionResult.FromSuccess()
                : hasAdminRole
                    ? PreconditionResult.FromSuccess()
                    : PreconditionResult.FromError(
                        $"Эта команда временно выключена.");
        }
    }
}