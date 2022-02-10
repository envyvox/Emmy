using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventCreated(SocketGuildEvent SocketGuildEvent) : IRequest;

    public class OnGuildScheduledEventCreatedHandler : IRequestHandler<OnGuildScheduledEventCreated>
    {
        private readonly ILogger<OnGuildScheduledEventCreatedHandler> _logger;

        public OnGuildScheduledEventCreatedHandler(ILogger<OnGuildScheduledEventCreatedHandler> logger)
        {
            _logger = logger;
        }

        public async Task<Unit> Handle(OnGuildScheduledEventCreated request, CancellationToken ct)
        {
            _logger.LogInformation(
                "[ScheduledEvent] Event {@Event} created",
                request.SocketGuildEvent);

            return await Task.FromResult(Unit.Value);
        }
    }
}