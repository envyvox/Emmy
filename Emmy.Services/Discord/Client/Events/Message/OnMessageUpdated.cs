using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.Message
{
    public record OnMessageUpdated(
            Cacheable<IMessage, ulong> OldMessage,
            SocketMessage NewMessage,
            ISocketMessageChannel SocketMessageChannel)
        : IRequest;

    public class OnMessageUpdatedHandler : IRequestHandler<OnMessageUpdated>
    {
        public async Task<Unit> Handle(OnMessageUpdated request, CancellationToken ct)
        {
            // ignored
            return await Task.FromResult(Unit.Value);
        }
    }
}