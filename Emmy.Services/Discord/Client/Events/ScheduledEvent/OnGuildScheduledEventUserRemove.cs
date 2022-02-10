using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventUserRemove(
            Cacheable<SocketUser, RestUser, IUser, ulong> SocketUser,
            SocketGuildEvent SocketGuildEvent)
        : IRequest;

    public class OnGuildScheduledEventUserRemoveHandler : IRequestHandler<OnGuildScheduledEventUserRemove>
    {
        private readonly ILogger<OnGuildScheduledEventUserRemoveHandler> _logger;

        public OnGuildScheduledEventUserRemoveHandler(ILogger<OnGuildScheduledEventUserRemoveHandler> logger)
        {
            _logger = logger;
        }

        public async Task<Unit> Handle(OnGuildScheduledEventUserRemove request, CancellationToken ct)
        {
            _logger.LogInformation(
                "[ScheduledEvent] User {UserId} unsubscribed from event {@Event}",
                request.SocketUser.Id, request.SocketGuildEvent);

            return await Task.FromResult(Unit.Value);
        }
    }
}