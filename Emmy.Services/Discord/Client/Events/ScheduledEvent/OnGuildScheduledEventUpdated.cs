using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventUpdated(
            Cacheable<SocketGuildEvent, ulong> OldSocketGuildEvent,
            SocketGuildEvent NewSocketGuildEvent)
        : IRequest;

    public class OnGuildScheduledEventUpdatedHandler : IRequestHandler<OnGuildScheduledEventUpdated>
    {
        public async Task<Unit> Handle(OnGuildScheduledEventUpdated request, CancellationToken ct)
        {
            // ignored
            return await Task.FromResult(Unit.Value);
        }
    }
}