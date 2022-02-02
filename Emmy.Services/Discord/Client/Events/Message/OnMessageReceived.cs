using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Emmy.Data.Enums;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.CommunityDesc.Commands;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Extensions;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Statistic.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Client.Events.Message
{
    public record OnMessageReceived(SocketMessage SocketMessage) : IRequest;

    public class OnMessageReceivedHandler : IRequestHandler<OnMessageReceived>
    {
        private readonly IMediator _mediator;

        public OnMessageReceivedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(OnMessageReceived request, CancellationToken cancellationToken)
        {
            if (request.SocketMessage.Author.IsBot) return await Task.FromResult(Unit.Value);

            var channels = DiscordRepository.Channels;
            var communityDescChannels = channels.GetCommunityDescChannels();

            if (communityDescChannels.Contains(request.SocketMessage.Channel.Id))
            {
                var hasAttachment =
                    request.SocketMessage.Attachments.Count == 1 ||
                    request.SocketMessage.Content.Contains("http");

                if (hasAttachment)
                {
                    await AddVotes((IUserMessage) request.SocketMessage);
                    await _mediator.Send(new CreateContentMessageCommand(
                        (long) request.SocketMessage.Author.Id, (long) request.SocketMessage.Channel.Id,
                        (long) request.SocketMessage.Id));
                }
                else
                {
                    await DeleteMessage(request.SocketMessage);
                }
            }

            if (request.SocketMessage.Channel.Id == channels[Channel.Suggestions].Id)
            {
                await AddVotes((IUserMessage) request.SocketMessage);
            }

            if (request.SocketMessage.Channel.Id == channels[Channel.Chat].Id)
            {
                var user = await _mediator.Send(new GetUserQuery((long) request.SocketMessage.Author.Id));

                await _mediator.Send(new AddStatisticToUserCommand(user.Id, Statistic.Messages));
                // todo check achievements
                // await _mediator.Send(new CheckAchievementInUserCommand(user.Id, AchievementType.FirstMessage));
            }

            if (request.SocketMessage.Channel.Id == channels[Channel.Commands].Id)
            {
                await DeleteMessage(request.SocketMessage);
            }

            return Unit.Value;
        }

        private async Task AddVotes(IUserMessage message)
        {
            var emotes = DiscordRepository.Emotes;

            await message.AddReactionsAsync(new IEmote[]
            {
                Parse(emotes.GetEmote("Like")),
                Parse(emotes.GetEmote("Dislike"))
            });
        }

        private async Task DeleteMessage(SocketMessage message)
        {
            // delay cuz discord cache
            await Task.Delay(1000);
            await message.DeleteAsync();
        }
    }
}