using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Guild.Models;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using ChannelType = Emmy.Data.Enums.Discord.ChannelType;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record SyncChannelsCommand : IRequest;

    public class SyncChannelsHandler : IRequestHandler<SyncChannelsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SyncChannelsHandler> _logger;

        public SyncChannelsHandler(
            IMediator mediator,
            ILogger<SyncChannelsHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(SyncChannelsCommand request, CancellationToken ct)
        {
            var loadedChannels = DiscordRepository.Channels;
            var channels = Enum
                .GetValues(typeof(Channel))
                .Cast<Channel>()
                .ToArray();

            if (loadedChannels.Count < channels.Length)
            {
                var guild = await _mediator.Send(new GetSocketGuildQuery());

                foreach (var channel in channels)
                {
                    if (loadedChannels.ContainsKey(channel)) continue;

                    var chan = guild.Channels.FirstOrDefault(x => x.Name == channel.Name());

                    if (chan is null)
                    {
                        switch (channel.Type())
                        {
                            case ChannelType.Text:

                                var textChannel = await guild.CreateTextChannelAsync(channel.Name(), x =>
                                {
                                    x.CategoryId = loadedChannels.ContainsKey(channel.Parent())
                                        ? loadedChannels[channel.Parent()].Id
                                        : Optional<ulong?>.Unspecified;
                                });

                                loadedChannels.Add(channel, new ChannelDto(textChannel.Id, channel));

                                break;
                            case ChannelType.Voice:

                                var voiceChannel = await guild.CreateVoiceChannelAsync(channel.Name(), x =>
                                {
                                    x.CategoryId = loadedChannels.ContainsKey(channel.Parent())
                                        ? loadedChannels[channel.Parent()].Id
                                        : Optional<ulong?>.Unspecified;
                                });

                                loadedChannels.Add(channel, new ChannelDto(voiceChannel.Id, channel));

                                break;
                            case ChannelType.Category:

                                var categoryChannel = await guild.CreateCategoryChannelAsync(channel.Name());

                                loadedChannels.Add(channel, new ChannelDto(categoryChannel.Id, channel));

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        loadedChannels.Add(channel, new ChannelDto(chan.Id, channel));
                    }
                }
            }

            _logger.LogInformation(
                "Channels sync completed");

            return Unit.Value;
        }
    }
}