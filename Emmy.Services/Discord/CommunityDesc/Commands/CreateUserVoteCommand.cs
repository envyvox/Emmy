using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.Discord;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.CommunityDesc.Commands
{
    public record CreateUserVoteCommand(long UserId, Guid ContentMessageId, Vote Vote) : IRequest;

    public class CreateUserVoteHandler : IRequestHandler<CreateUserVoteCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateUserVoteHandler> _logger;
        private readonly AppDbContext _db;

        public CreateUserVoteHandler(
            DbContextOptions options,
            IMediator mediator,
            ILogger<CreateUserVoteHandler> logger)
        {
            _db = new AppDbContext(options);
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateUserVoteCommand request, CancellationToken cancellationToken)
        {
            var exist = await _db.ContentVotes
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.ContentMessageId == request.ContentMessageId &&
                    x.Vote == request.Vote);

            if (exist)
            {
                throw new Exception(
                    $"vote {request.Vote.ToString()} from user {request.UserId} on content message {request.ContentMessageId} already exist");
            }

            var created = await _db.CreateEntity(new ContentVote
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ContentMessageId = request.ContentMessageId,
                Vote = request.Vote,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created user vote entity {@Entity}",
                created);

            if (request.Vote == Vote.Like)
                await _mediator.Send(new CheckContentMessageLikesCommand(request.ContentMessageId));
            if (request.Vote == Vote.Dislike)
                await _mediator.Send(new CheckContentMessageDislikesCommand(request.ContentMessageId));

            return Unit.Value;
        }
    }
}