using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventCancelled(SocketGuildEvent SocketGuildEvent) : IRequest;

    public class OnGuildScheduledEventCancelledHandler : IRequestHandler<OnGuildScheduledEventCancelled>
    {
        private readonly ILogger<OnGuildScheduledEventCancelledHandler> _logger;

        public OnGuildScheduledEventCancelledHandler(ILogger<OnGuildScheduledEventCancelledHandler> logger)
        {
            _logger = logger;
        }

        public async Task<Unit> Handle(OnGuildScheduledEventCancelled request, CancellationToken ct)
        {
            _logger.LogInformation(
                "[ScheduledEvent] Event {@Event} was cancelled",
                request.SocketGuildEvent);

            return await Task.FromResult(Unit.Value);
        }
    }
}