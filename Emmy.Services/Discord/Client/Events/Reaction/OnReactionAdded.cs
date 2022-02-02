using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Emmy.Data.Enums;
using Emmy.Services.Discord.CommunityDesc.Commands;
using Emmy.Services.Discord.CommunityDesc.Queries;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Extensions;
using Emmy.Services.Extensions;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Client.Events.Reaction
{
    public record OnReactionAdded(
            Cacheable<IUserMessage, ulong> UserMessage,
            Cacheable<IMessageChannel, ulong> MessageChannel,
            SocketReaction SocketReaction)
        : IRequest;

    public class OnReactionAddedHandler : IRequestHandler<OnReactionAdded>
    {
        private readonly IMediator _mediator;

        public OnReactionAddedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(OnReactionAdded request, CancellationToken ct)
        {
            if (request.SocketReaction.User.Value.IsBot) return Unit.Value;

            var msg = await request.UserMessage.GetOrDownloadAsync();
            var channels = DiscordRepository.Channels;
            var communityDescChannels = channels.GetCommunityDescChannels();

            if (communityDescChannels.Contains(request.MessageChannel.Id))
            {
                if (request.SocketReaction.Emote.Name is not ("Like" or "Dislike")) return Unit.Value;

                var contentMessage = await _mediator.Send(new GetContentMessageByParamsQuery(
                    (long) request.MessageChannel.Id, (long) request.UserMessage.Id));

                if (request.SocketReaction.UserId == (ulong) contentMessage.User.Id)
                {
                    await msg.RemoveReactionAsync(request.SocketReaction.Emote, request.SocketReaction.UserId);
                }
                else
                {
                    var vote = request.SocketReaction.Emote.Name == "Like" ? Vote.Like : Vote.Dislike;
                    var antiVote = vote == Vote.Like ? Vote.Dislike : Vote.Like;
                    var userVotes = await _mediator.Send(new GetUserVotesOnMessageQuery(
                        (long) request.SocketReaction.UserId, contentMessage.Id));

                    if (userVotes.ContainsKey(antiVote) && userVotes[antiVote].IsActive)
                    {
                        var emotes = DiscordRepository.Emotes;

                        await msg.RemoveReactionAsync(
                            antiVote == Vote.Like
                                ? Parse(emotes.GetEmote("Like"))
                                : Parse(emotes.GetEmote("Dislike")),
                            request.SocketReaction.UserId);
                    }

                    if (userVotes.ContainsKey(vote))
                    {
                        await _mediator.Send(new ActivateUserVoteCommand(
                            (long) request.SocketReaction.UserId, contentMessage.Id, vote));
                    }
                    else
                    {
                        await _mediator.Send(new CreateUserVoteCommand(
                            (long) request.SocketReaction.UserId, contentMessage.Id, vote));
                    }
                }
            }

            return Unit.Value;
        }
    }
}