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
using MediatR;

namespace Emmy.Services.Discord.CommunityDesc.Commands
{
    public record CheckContentMessageDislikesCommand(Guid ContentMessageId) : IRequest;

    public class CheckContentMessageDislikesHandler : IRequestHandler<CheckContentMessageDislikesCommand>
    {
        private readonly IMediator _mediator;

        public CheckContentMessageDislikesHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CheckContentMessageDislikesCommand request, CancellationToken cancellationToken)
        {
            var messageDislikes = await _mediator.Send(new GetContentMessageVotesQuery(
                request.ContentMessageId, Vote.Dislike));

            if (messageDislikes.Count >= 5)
            {
                var message = await _mediator.Send(new GetUserMessageQuery(
                    (ulong) messageDislikes[0].Message.ChannelId, (ulong) messageDislikes[0].Message.MessageId));
                var emotes = DiscordRepository.Emotes;

                var embed = new EmbedBuilder()
                    .WithAuthor("Оповещение от доски сообщества")
                    .WithDescription(
                        $"Твоя публикация собрала {emotes.GetEmote("Dislike")} 5 дизлайков и была автоматически удалена из <#{message.Channel.Id}>.")
                    .WithImageUrl(message.Attachments.First().Url);

                await _mediator.Send(new SendEmbedToUserCommand(message.Author.Id, embed));
                await message.DeleteAsync();
            }

            return Unit.Value;
        }
    }
}