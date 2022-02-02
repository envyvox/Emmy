using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventCancelled(SocketGuildEvent SocketGuildEvent) : IRequest;

    public class OnGuildScheduledEventCancelledHandler : IRequestHandler<OnGuildScheduledEventCancelled>
    {
        public async Task<Unit> Handle(OnGuildScheduledEventCancelled request, CancellationToken ct)
        {
            // ignored
            return await Task.FromResult(Unit.Value);
        }
    }
}