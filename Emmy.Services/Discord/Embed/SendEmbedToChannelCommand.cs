using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Embed
{
    public record SendEmbedToChannelCommand(
            Channel Channel,
            EmbedBuilder EmbedBuilder,
            MessageComponent Component = null,
            string Message = "")
        : IRequest;

    public class SendEmbedToChannelHandler : IRequestHandler<SendEmbedToChannelCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SendEmbedToChannelHandler> _logger;

        public SendEmbedToChannelHandler(
            IMediator mediator,
            ILogger<SendEmbedToChannelHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(SendEmbedToChannelCommand request, CancellationToken ct)
        {
            var channels = DiscordRepository.Channels;
            var channel = await _mediator.Send(new GetSocketTextChannelQuery(channels[request.Channel].Id));

            try
            {
                await channel.SendMessageAsync(request.Message,
                    embed: request.EmbedBuilder.Build(),
                    components: request.Component);

                _logger.LogInformation(
                    "Sended a message in channel {ChannelType}",
                    request.Channel.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Can't send message to channel {ChannelType}",
                    request.Channel.ToString());
            }

            return Unit.Value;
        }
    }
}