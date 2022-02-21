using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums;
using Emmy.Services.Discord.CommunityDesc.Queries;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.CommunityDesc.Commands
{
    public record CheckContentMessageDislikesCommand(Guid ContentMessageId) : IRequest;

    public class CheckContentMessageDislikesHandler : IRequestHandler<CheckContentMessageDislikesCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CheckContentMessageDislikesHandler> _logger;

        public CheckContentMessageDislikesHandler(
            IMediator mediator,
            ILogger<CheckContentMessageDislikesHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(CheckContentMessageDislikesCommand request, CancellationToken cancellationToken)
        {
            var messageDislikes = await _mediator.Send(new GetContentMessageVotesQuery(
                request.ContentMessageId, Vote.Dislike));

            if (messageDislikes.Count >= 5)
            {
                var message = await _mediator.Send(new GetUserMessageQuery(
                    (ulong) messageDislikes[0].ContentMessage.ChannelId, (ulong) messageDislikes[0].ContentMessage.MessageId));
                var emotes = DiscordRepository.Emotes;
                var user = await _mediator.Send(new GetUserQuery((long) message.Author.Id));

                var embed = new EmbedBuilder()
                    .WithAuthor("Оповещение от доски сообщества", message.Author.GetAvatarUrl())
                    .WithDescription(
                        $"{message.Author.Mention.AsGameMention(user.Title)}, " +
                        $"твоя публикация собрала {emotes.GetEmote("Dislike")} 5 дизлайков и была " +
                        $"автоматически удалена из <#{message.Channel.Id}>.")
                    .WithImageUrl(message.Attachments.First().Url);

                await _mediator.Send(new SendEmbedToUserCommand(message.Author.Id, embed));
                await message.DeleteAsync();

                _logger.LogInformation(
                    "Content message {MessageId} in channel {ChannelId} got 5 dislikes and deleted",
                    message.Id, message.Channel.Id);
            }

            return Unit.Value;
        }
    }
}