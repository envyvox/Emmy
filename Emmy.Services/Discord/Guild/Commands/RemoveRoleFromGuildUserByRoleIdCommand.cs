using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Services.Discord.Guild.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record RemoveRoleFromGuildUserByRoleIdCommand(ulong UserId, ulong RoleId) : IRequest;

    public class RemoveRoleFromGuildUserByRoleIdHandler : IRequestHandler<RemoveRoleFromGuildUserByRoleIdCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RemoveRoleFromGuildUserByRoleIdHandler> _logger;

        public RemoveRoleFromGuildUserByRoleIdHandler(
            IMediator mediator,
            ILogger<RemoveRoleFromGuildUserByRoleIdHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveRoleFromGuildUserByRoleIdCommand request, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetSocketGuildUserQuery(request.UserId));

            try
            {
                await user.RemoveRoleAsync(request.RoleId);

                _logger.LogInformation(
                    "Removed role {RoleId} from user {UserId}",
                    request.RoleId, request.UserId);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Can't remove role {RoleId} from user {UserId}",
                    request.RoleId, request.UserId);
            }

            return Unit.Value;
        }
    }
}