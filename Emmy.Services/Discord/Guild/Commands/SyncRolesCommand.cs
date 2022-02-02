using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Guild.Models;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record SyncRolesCommand : IRequest;

    public class SyncRolesHandler : IRequestHandler<SyncRolesCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SyncRolesHandler> _logger;

        public SyncRolesHandler(
            IMediator mediator,
            ILogger<SyncRolesHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(SyncRolesCommand request, CancellationToken ct)
        {
            var loadedRoles = DiscordRepository.Roles;
            var roles = Enum
                .GetValues(typeof(Data.Enums.Discord.Role))
                .Cast<Data.Enums.Discord.Role>()
                .ToArray();

            if (loadedRoles.Count < roles.Length)
            {
                var guild = await _mediator.Send(new GetSocketGuildQuery());

                foreach (var role in roles)
                {
                    if (loadedRoles.ContainsKey(role)) continue;

                    var roleInGuild = guild.Roles.FirstOrDefault(x => x.Name == role.Name());
                    ulong roleId;

                    if (roleInGuild is null)
                    {
                        var newRole = await guild.CreateRoleAsync(
                            name: role.Name(),
                            permissions: null,
                            color: new Color(uint.Parse(role.Color(), NumberStyles.HexNumber)),
                            isHoisted: false,
                            options: null);

                        roleId = newRole.Id;
                    }
                    else
                    {
                        roleId = roleInGuild.Id;
                    }

                    loadedRoles.Add(role, new RoleDto(roleId, role));
                }
            }

            _logger.LogInformation(
                "Roles sync completed");

            return Unit.Value;
        }
    }
}