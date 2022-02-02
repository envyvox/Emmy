using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.Discord;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.CommunityDesc.Commands
{
    public record CreateUserVoteCommand(long UserId, Guid ContentMessageId, Vote Vote) : IRequest;

    public class CreateUserVoteHandler : IRequestHandler<CreateUserVoteCommand>
    {
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public CreateUserVoteHandler(
            DbContextOptions options,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateUserVoteCommand request, CancellationToken cancellationToken)
        {
            var exist = await _db.ContentVotes
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.ContentMessageId == request.ContentMessageId);

            if (exist)
            {
                throw new Exception(
                    $"vote from user {request.UserId} on content message {request.ContentMessageId} already exist");
            }

            await _db.CreateEntity(new ContentVote
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ContentMessageId = request.ContentMessageId,
                Vote = request.Vote,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            if (request.Vote == Vote.Like)
                await _mediator.Send(new CheckContentMessageLikesCommand(request.ContentMessageId));
            if (request.Vote == Vote.Dislike)
                await _mediator.Send(new CheckContentMessageDislikesCommand(request.ContentMessageId));

            return Unit.Value;
        }
    }
}