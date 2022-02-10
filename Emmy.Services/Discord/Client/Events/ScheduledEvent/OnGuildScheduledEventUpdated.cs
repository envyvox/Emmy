using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventUpdated(
            Cacheable<SocketGuildEvent, ulong> OldSocketGuildEvent,
            SocketGuildEvent NewSocketGuildEvent)
        : IRequest;

    public class OnGuildScheduledEventUpdatedHandler : IRequestHandler<OnGuildScheduledEventUpdated>
    {
        private readonly ILogger<OnGuildScheduledEventUpdatedHandler> _logger;

        public OnGuildScheduledEventUpdatedHandler(ILogger<OnGuildScheduledEventUpdatedHandler> logger)
        {
            _logger = logger;
        }

        public async Task<Unit> Handle(OnGuildScheduledEventUpdated request, CancellationToken ct)
        {
            _logger.LogInformation(
                "[ScheduledEvent] Event updated from {@OldEvent} to {@NewEvent}",
                request.OldSocketGuildEvent.GetOrDownloadAsync(), request.NewSocketGuildEvent);

            return await Task.FromResult(Unit.Value);
        }
    }
}