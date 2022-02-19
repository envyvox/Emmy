using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Emmy.Data.Enums;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Client.Events.User
{
    public record OnUserJoined(SocketGuildUser SocketGuildUser) : IRequest;

    public class OnUserJoinedHandler : IRequestHandler<OnUserJoined>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OnUserJoinedHandler> _logger;

        public OnUserJoinedHandler(
            IMediator mediator,
            ILogger<OnUserJoinedHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(OnUserJoined request, CancellationToken ct)
        {
            _logger.LogInformation(
                "User {UserId} joined a guild",
                request.SocketGuildUser.Id);

            var user = await _mediator.Send(new GetUserQuery((long) request.SocketGuildUser.Id));

            await _mediator.Send(new RenameGuildUserCommand(
                request.SocketGuildUser.Id, request.SocketGuildUser.Username));
            await _mediator.Send(new AddRoleToGuildUserByRoleTypeCommand(
                request.SocketGuildUser.Id, user.Fraction.Role()));

            if (user.OnGuild is false)
            {
                await _mediator.Send(new UpdateUserOnGuildCommand(user.Id, true));
            }

            if (user.IsPremium &&
                request.SocketGuildUser.Roles.All(x => x.Name != Data.Enums.Discord.Role.Premium.Name()))
            {
                await _mediator.Send(new AddRoleToGuildUserByRoleTypeCommand(
                    request.SocketGuildUser.Id, Data.Enums.Discord.Role.Premium));
            }

            if (user.Gender is not Gender.None)
            {
                await _mediator.Send(new AddRoleToGuildUserByRoleTypeCommand(
                    request.SocketGuildUser.Id, user.Gender.Role()));
            }

            return Unit.Value;
        }
    }
}