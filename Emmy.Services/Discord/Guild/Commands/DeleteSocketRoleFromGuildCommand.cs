using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Services.Discord.Guild.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record DeleteSocketRoleFromGuildCommand(ulong RoleId) : IRequest;

    public class DeleteSocketRoleFromGuildHandler : IRequestHandler<DeleteSocketRoleFromGuildCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteSocketRoleFromGuildHandler> _logger;

        public DeleteSocketRoleFromGuildHandler(
            IMediator mediator,
            ILogger<DeleteSocketRoleFromGuildHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteSocketRoleFromGuildCommand request, CancellationToken ct)
        {
            var socketRole = await _mediator.Send(new GetSocketRoleQuery(request.RoleId));

            try
            {
                await socketRole.DeleteAsync();

                _logger.LogInformation(
                    "Deleted socket role {@Role} from guild",
                    socketRole);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot delete socket role {RoleId} from guild",
                    request.RoleId);
            }

            return Unit.Value;
        }
    }
}