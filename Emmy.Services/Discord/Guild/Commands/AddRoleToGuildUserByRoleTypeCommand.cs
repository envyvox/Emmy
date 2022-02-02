using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record AddRoleToGuildUserByRoleTypeCommand(ulong UserId, Data.Enums.Discord.Role Role) : IRequest;

    public class AddRoleToGuildUserByRoleTypeHandler : IRequestHandler<AddRoleToGuildUserByRoleTypeCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AddRoleToGuildUserByRoleTypeHandler> _logger;

        public AddRoleToGuildUserByRoleTypeHandler(
            IMediator mediator,
            ILogger<AddRoleToGuildUserByRoleTypeHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddRoleToGuildUserByRoleTypeCommand request, CancellationToken ct)
        {
            var roles = DiscordRepository.Roles;
            var guild = await _mediator.Send(new GetSocketGuildQuery());
            var user = await _mediator.Send(new GetSocketGuildUserQuery(request.UserId));

            try
            {
                await user.AddRoleAsync(guild.GetRole(roles[request.Role].Id));

                _logger.LogInformation(
                    "Added role {Role} to user {UserId}",
                    request.Role.ToString(), request.UserId);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Can't add role {Role} to user {UserId}",
                    request.Role.ToString(), request.UserId);
            }

            return Unit.Value;
        }
    }
}