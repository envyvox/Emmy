using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventCompleted(SocketGuildEvent SocketGuildEvent) : IRequest;

    public class OnGuildScheduledEventCompletedHandler : IRequestHandler<OnGuildScheduledEventCompleted>
    {
        public async Task<Unit> Handle(OnGuildScheduledEventCompleted request, CancellationToken ct)
        {
            // ignored
            return await Task.FromResult(Unit.Value);
        }
    }
}