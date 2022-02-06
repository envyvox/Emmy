using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Services.Discord.CommunityDesc.Commands;
using Emmy.Services.Discord.Guild.Extensions;
using Emmy.Services.Extensions;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.Message
{
    public record OnMessageDeleted(
            Cacheable<IMessage, ulong> DeletedMessage,
            Cacheable<IMessageChannel, ulong> MessageChannel)
        : IRequest;

    public class OnMessageDeletedHandler : IRequestHandler<OnMessageDeleted>
    {
        private readonly IMediator _mediator;

        public OnMessageDeletedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(OnMessageDeleted request, CancellationToken ct)
        {
            var channels = DiscordRepository.Channels;
            var communityDescChannels = channels.GetCommunityDescChannels();

            if (communityDescChannels.Contains(request.MessageChannel.Id))
            {
                await _mediator.Send(new DeleteContentMessageCommand(
                    (long) request.MessageChannel.Id, (long) request.DeletedMessage.Id));
            }

            return Unit.Value;
        }
    }
}