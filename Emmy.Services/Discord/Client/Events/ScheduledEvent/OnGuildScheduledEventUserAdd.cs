using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventUserAdd(
            Cacheable<SocketUser, RestUser, IUser, ulong> SocketUser,
            SocketGuildEvent SocketGuildEvent)
        : IRequest;
    
    public class OnGuildScheduledEventUserAddHandler : IRequestHandler<OnGuildScheduledEventUserAdd>
    {
        public async Task<Unit> Handle(OnGuildScheduledEventUserAdd request, CancellationToken ct)
        {
            // ignored
            return await Task.FromResult(Unit.Value);
        }
    }
}