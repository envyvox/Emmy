using System.Threading;
using System.Threading.Tasks;
using Emmy.Services.Discord.Client;
using Emmy.Services.Discord.Emote.Models;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Emote.Commands
{
    public record SyncEmotesCommand : IRequest;

    public class SyncEmotesHandler : IRequestHandler<SyncEmotesCommand>
    {
        private readonly IDiscordClientService _discordClientService;
        private readonly ILogger<SyncEmotesHandler> _logger;

        public SyncEmotesHandler(
            IDiscordClientService discordClientService,
            ILogger<SyncEmotesHandler> logger)
        {
            _discordClientService = discordClientService;
            _logger = logger;
        }

        public async Task<Unit> Handle(SyncEmotesCommand request, CancellationToken ct)
        {
            var socketClient = await _discordClientService.GetSocketClient();
            var emotes = DiscordRepository.Emotes;

            foreach (var guild in socketClient.Guilds)
            {
                foreach (var emote in guild.Emotes)
                {
                    if (emotes.ContainsKey(emote.Name)) continue;

                    emotes.Add(emote.Name, new EmoteDto(emote.Id, emote.Name, emote.ToString()));
                }
            }

            _logger.LogInformation(
                "Emotes sync completed");

            return Unit.Value;
        }
    }
}