using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record RemoveRoleFromGuildUserByRoleTypeCommand(ulong UserId, Data.Enums.Discord.Role Role) : IRequest;

    public class RemoveRoleFromGuildUserByRoleTypeHandler : IRequestHandler<RemoveRoleFromGuildUserByRoleTypeCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RemoveRoleFromGuildUserByRoleTypeHandler> _logger;

        public RemoveRoleFromGuildUserByRoleTypeHandler(
            IMediator mediator,
            ILogger<RemoveRoleFromGuildUserByRoleTypeHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveRoleFromGuildUserByRoleTypeCommand request, CancellationToken ct)
        {
            var roles = DiscordRepository.Roles;
            var guild = await _mediator.Send(new GetSocketGuildQuery());
            var user = await _mediator.Send(new GetSocketGuildUserQuery(request.UserId));

            try
            {
                await user.RemoveRoleAsync(guild.GetRole(roles[request.Role].Id));

                _logger.LogInformation(
                    "Removed role {Role} from user {UserId}",
                    request.Role.ToString(), request.UserId);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Can't remove role {Role} from user {UserId}",
                    request.Role.ToString(), request.UserId);
            }

            return Unit.Value;
        }
    }
}