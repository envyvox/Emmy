using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Queries
{
    public record GetSocketTextChannelQuery(ulong Id) : IRequest<SocketTextChannel>;

    public class GetSocketTextChannelHandler : IRequestHandler<GetSocketTextChannelQuery, SocketTextChannel>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetSocketTextChannelHandler> _logger;

        public GetSocketTextChannelHandler(
            IMediator mediator,
            ILogger<GetSocketTextChannelHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<SocketTextChannel> Handle(GetSocketTextChannelQuery request, CancellationToken ct)
        {
            var guild = await _mediator.Send(new GetSocketGuildQuery());

            try
            {
                return guild.GetTextChannel(request.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Can't get text channel {ChannelId}",
                    request.Id);

                throw;
            }
        }
    }
}