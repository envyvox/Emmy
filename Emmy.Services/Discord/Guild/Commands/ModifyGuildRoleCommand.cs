using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Services.Discord.Guild.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record ModifyGuildRoleCommand(ulong RoleId, string Name, string Color) : IRequest;

    public class ModifyGuildRoleHandler : IRequestHandler<ModifyGuildRoleCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ModifyGuildRoleHandler> _logger;

        public ModifyGuildRoleHandler(
            IMediator mediator,
            ILogger<ModifyGuildRoleHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(ModifyGuildRoleCommand request, CancellationToken ct)
        {
            var guildRole = await _mediator.Send(new GetSocketRoleQuery(request.RoleId));

            try
            {
                await guildRole.ModifyAsync(x =>
                {
                    x.Name = request.Name;
                    x.Color = new Color(uint.Parse(request.Color, NumberStyles.HexNumber));
                });

                _logger.LogInformation(
                    "Updated socket guild role {RoleId} with name {Name} and color {Color}",
                    request.RoleId, request.Name, request.Color);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot modify role {RoleI} and set name {Name} and color {Color}",
                    request.RoleId, request.Name, request.Color);
            }

            return Unit.Value;
        }
    }
}