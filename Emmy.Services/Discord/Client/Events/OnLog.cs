using System.Threading;
using System.Threading.Tasks;
using Discord;
using MediatR;

namespace Emmy.Services.Discord.Client.Events
{
    public record OnLog(LogMessage LogMessage) : IRequest;

    public class OnLogHandler : IRequestHandler<OnLog>
    {
        public async Task<Unit> Handle(OnLog request, CancellationToken ct)
        {
            // ignored
            return await Task.FromResult(Unit.Value);
        }
    }
}