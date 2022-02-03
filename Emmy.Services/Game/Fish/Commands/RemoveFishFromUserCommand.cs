using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Fish.Commands
{
    public record RemoveFishFromUserCommand(long UserId, Guid FishId, uint Amount) : IRequest;

    public class RemoveFishFromUserHandler : IRequestHandler<RemoveFishFromUserCommand>
    {
        private readonly ILogger<RemoveFishFromUserHandler> _logger;
        private readonly AppDbContext _db;

        public RemoveFishFromUserHandler(
            DbContextOptions options,
            ILogger<RemoveFishFromUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(RemoveFishFromUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserFishes
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.FishId == request.FishId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have fish {request.FishId}");
            }

            entity.Amount -= request.Amount;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Removed from user {UserId} fish {FishId} amount {Amount}",
                request.UserId, request.FishId, request.Amount);

            return Unit.Value;
        }
    }
}