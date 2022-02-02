﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.CommunityDesc.Queries;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Role.Commands;
using Emmy.Services.Extensions;
using MediatR;

namespace Emmy.Services.Discord.CommunityDesc.Commands
{
    public record CheckContentMessageLikesCommand(Guid ContentMessageId) : IRequest;

    public class CheckContentMessageLikesHandler : IRequestHandler<CheckContentMessageLikesCommand>
    {
        private readonly IMediator _mediator;

        public CheckContentMessageLikesHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CheckContentMessageLikesCommand request, CancellationToken cancellationToken)
        {
            var contentMessage = await _mediator.Send(new GetContentMessageQuery(
                request.ContentMessageId));
            var authorLikes = await _mediator.Send(new GetContentAuthorVotesCountQuery(
                contentMessage.User.Id, Vote.Like));

            if (authorLikes % 500 == 0)
            {
                var emotes = DiscordRepository.Emotes;
                var roles = DiscordRepository.Roles;

                await _mediator.Send(new AddRoleToGuildUserByRoleTypeCommand(
                    (ulong) contentMessage.User.Id, Data.Enums.Discord.Role.ContentProvider));
                await _mediator.Send(new AddRoleToUserCommand(
                    contentMessage.User.Id, (long) roles[Data.Enums.Discord.Role.ContentProvider].Id,
                    TimeSpan.FromDays(30)));

                var embed = new EmbedBuilder()
                    .WithAuthor("Оповещение от доски сообщества")
                    .WithDescription(
                        $"Твои публикации в **доске сообщества** были {emotes.GetEmote("Like")} оценены, " +
                        $"за что ты получаешь роль **{Data.Enums.Discord.Role.ContentProvider.Name()}** на 30 дней.");

                await _mediator.Send(new SendEmbedToUserCommand((ulong) contentMessage.User.Id, embed));
            }

            return Unit.Value;
        }
    }
}