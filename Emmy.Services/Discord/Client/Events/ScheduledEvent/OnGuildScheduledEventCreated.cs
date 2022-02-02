using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventCreated(SocketGuildEvent SocketGuildEvent) : IRequest;

    public class OnGuildScheduledEventCreatedHandler : IRequestHandler<OnGuildScheduledEventCreated>
    {
        public async Task<Unit> Handle(OnGuildScheduledEventCreated request, CancellationToken ct)
        {
            // ignored
            return await Task.FromResult(Unit.Value);
        }
    }
}