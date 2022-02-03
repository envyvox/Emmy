using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Services.Discord.Guild.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record AddRoleToGuildUserByRoleIdCommand(ulong UserId, ulong RoleId) : IRequest;

    public class AddRoleToGuildUserByRoleIdHandler : IRequestHandler<AddRoleToGuildUserByRoleIdCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AddRoleToGuildUserByRoleIdHandler> _logger;

        public AddRoleToGuildUserByRoleIdHandler(
            IMediator mediator,
            ILogger<AddRoleToGuildUserByRoleIdHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddRoleToGuildUserByRoleIdCommand request, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetSocketGuildUserQuery(request.UserId));

            try
            {
                await user!.AddRoleAsync(request.RoleId);

                _logger.LogInformation(
                    "Added role {RoleId} to user {UserId}",
                    request.RoleId, request.UserId);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Can't add role {RoleId} to user {UserId}",
                    request.RoleId, request.UserId);
            }

            return Unit.Value;
        }
    }
}