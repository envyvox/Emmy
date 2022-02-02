using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Emmy.Data.Enums;
using Emmy.Services.Discord.CommunityDesc.Commands;
using Emmy.Services.Discord.CommunityDesc.Queries;
using Emmy.Services.Discord.Guild.Extensions;
using Emmy.Services.Extensions;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.Reaction
{
    public record OnReactionRemoved(
            Cacheable<IUserMessage, ulong> UserMessage,
            Cacheable<IMessageChannel, ulong> MessageChannel,
            SocketReaction SocketReaction)
        : IRequest;

    public class OnReactionRemovedHandler : IRequestHandler<OnReactionRemoved>
    {
        private readonly IMediator _mediator;

        public OnReactionRemovedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(OnReactionRemoved request, CancellationToken ct)
        {
            if (request.SocketReaction.User.Value.IsBot) return Unit.Value;

            var channels = DiscordRepository.Channels;
            var communityDescChannels = channels.GetCommunityDescChannels();

            if (communityDescChannels.Contains(request.MessageChannel.Id))
            {
                if (request.SocketReaction.Emote.Name is not ("Like" or "Dislike")) return Unit.Value;

                var contentMessage = await _mediator.Send(new GetContentMessageByParamsQuery(
                    (long) request.MessageChannel.Id, (long) request.UserMessage.Id));

                await _mediator.Send(new DeactivateUserVoteCommand(
                    (long) request.SocketReaction.UserId, contentMessage.Id,
                    request.SocketReaction.Emote.Name == "Like" ? Vote.Like : Vote.Dislike));
            }

            return Unit.Value;
        }
    }
}