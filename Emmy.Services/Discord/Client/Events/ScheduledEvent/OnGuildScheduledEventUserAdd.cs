using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventUserAdd(
            Cacheable<SocketUser, RestUser, IUser, ulong> SocketUser,
            SocketGuildEvent SocketGuildEvent)
        : IRequest;

    public class OnGuildScheduledEventUserAddHandler : IRequestHandler<OnGuildScheduledEventUserAdd>
    {
        private readonly ILogger<OnGuildScheduledEventUserAddHandler> _logger;

        public OnGuildScheduledEventUserAddHandler(ILogger<OnGuildScheduledEventUserAddHandler> logger)
        {
            _logger = logger;
        }

        public async Task<Unit> Handle(OnGuildScheduledEventUserAdd request, CancellationToken ct)
        {
            _logger.LogInformation(
                "[ScheduledEvent] User {UserId} subscribed to event {@Event}",
                request.SocketUser.Id, request.SocketGuildEvent);

            return await Task.FromResult(Unit.Value);
        }
    }
}