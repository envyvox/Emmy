#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Queries
{
    public record GetSocketGuildUserQuery(ulong UserId) : IRequest<SocketGuildUser?>;

    public class GetSocketGuildUserHandler : IRequestHandler<GetSocketGuildUserQuery, SocketGuildUser?>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetSocketGuildUserHandler> _logger;

        public GetSocketGuildUserHandler(
            IMediator mediator,
            ILogger<GetSocketGuildUserHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<SocketGuildUser?> Handle(GetSocketGuildUserQuery request, CancellationToken ct)
        {
            var socketGuild = await _mediator.Send(new GetSocketGuildQuery());

            await socketGuild.DownloadUsersAsync();

            var socketUser = socketGuild.GetUser(request.UserId);

            if (socketUser is null)
            {
                _logger.LogWarning(
                    "socket user {UserId} not found in guild {GuildId}",
                    request.UserId, socketGuild.Id);
            }

            return socketUser;
        }
    }
}