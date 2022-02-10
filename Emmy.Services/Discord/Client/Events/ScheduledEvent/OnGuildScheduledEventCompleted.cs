using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventCompleted(SocketGuildEvent SocketGuildEvent) : IRequest;

    public class OnGuildScheduledEventCompletedHandler : IRequestHandler<OnGuildScheduledEventCompleted>
    {
        private readonly ILogger<OnGuildScheduledEventCompletedHandler> _logger;

        public OnGuildScheduledEventCompletedHandler(ILogger<OnGuildScheduledEventCompletedHandler> logger)
        {
            _logger = logger;
        }

        public async Task<Unit> Handle(OnGuildScheduledEventCompleted request, CancellationToken ct)
        {
            _logger.LogInformation(
                "[ScheduledEvent] Event {@Event} completed",
                request.SocketGuildEvent);

            return await Task.FromResult(Unit.Value);
        }
    }
}