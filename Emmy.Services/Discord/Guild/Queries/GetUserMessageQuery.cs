using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Queries
{
    public record GetUserMessageQuery(ulong ChannelId, ulong MessageId) : IRequest<IUserMessage>;

    public class GetUserMessageHandler : IRequestHandler<GetUserMessageQuery, IUserMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetUserMessageHandler> _logger;

        public GetUserMessageHandler(
            IMediator mediator,
            ILogger<GetUserMessageHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IUserMessage> Handle(GetUserMessageQuery request, CancellationToken ct)
        {
            var channel = await _mediator.Send(new GetSocketTextChannelQuery(request.ChannelId));

            try
            {
                return (IUserMessage) await channel.GetMessageAsync(request.MessageId);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Can't get user message {MessageId}",
                    request.MessageId);

                throw;
            }
        }
    }
}