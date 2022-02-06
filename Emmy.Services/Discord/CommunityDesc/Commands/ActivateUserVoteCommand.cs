using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.CommunityDesc.Commands
{
    public record ActivateUserVoteCommand(long UserId, Guid ContentMessageId, Vote Vote) : IRequest;

    public class ActivateUserVoteHandler : IRequestHandler<ActivateUserVoteCommand>
    {
        private readonly ILogger<ActivateUserVoteHandler> _logger;
        private readonly AppDbContext _db;

        public ActivateUserVoteHandler(
            DbContextOptions options,
            ILogger<ActivateUserVoteHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(ActivateUserVoteCommand request, CancellationToken ct)
        {
            var entity = await _db.ContentVotes
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.ContentMessageId == request.ContentMessageId &&
                    x.Vote == request.Vote);

            if (entity is null)
            {
                throw new Exception(
                    "content vote with params " +
                    $"userId: {request.UserId}, " +
                    $"messageId: {request.ContentMessageId}, " +
                    $"vote: {request.Vote} " +
                    "not found");
            }

            entity.IsActive = true;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Activated user {UserId} {Vote} on content message {ContentMessageId}",
                request.UserId, request.Vote.ToString(), request.ContentMessageId);

            return Unit.Value;
        }
    }
}