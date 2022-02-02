using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Emmy.Services.Discord.Client;
using Emmy.Services.Discord.Extensions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Emmy.Services.Discord.Guild.Queries
{
    public record GetSocketGuildQuery(ulong GuildId = 0) : IRequest<SocketGuild>;

    public class GetSocketGuildHandler : IRequestHandler<GetSocketGuildQuery, SocketGuild>
    {
        private readonly IDiscordClientService _discordClientService;
        private readonly DiscordClientOptions _options;

        public GetSocketGuildHandler(
            IDiscordClientService discordClientService,
            IOptions<DiscordClientOptions> options)
        {
            _discordClientService = discordClientService;
            _options = options.Value;
        }

        public async Task<SocketGuild> Handle(GetSocketGuildQuery request, CancellationToken ct)
        {
            var guildId = request.GuildId == 0
                ? _options.GuildId
                : request.GuildId;
            var client = await _discordClientService.GetSocketClient();
            var guild = client.GetGuild(guildId);

            if (guild is null)
            {
                throw new Exception(
                    $"socket guild {guildId} was not found.");
            }

            return guild;
        }
    }
}