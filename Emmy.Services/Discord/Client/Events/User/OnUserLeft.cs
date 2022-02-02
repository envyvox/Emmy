using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Client.Events.User
{
    public record OnUserLeft(SocketGuild SocketGuild, SocketUser SocketUser) : IRequest;

    public class OnUserLeftHandler : IRequestHandler<OnUserLeft>
    {
        private readonly ILogger<OnUserLeftHandler> _logger;
        private readonly IMediator _mediator;

        public OnUserLeftHandler(
            ILogger<OnUserLeftHandler> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(OnUserLeft request, CancellationToken ct)
        {
            _logger.LogInformation(
                "User {UserId} left guild",
                request.SocketUser.Id);

            var user = await _mediator.Send(new GetUserQuery((long) request.SocketUser.Id));

            if (user.OnGuild is true)
            {
                await _mediator.Send(new UpdateUserOnGuildCommand(user.Id, false));
            }

            return Unit.Value;
        }
    }
}