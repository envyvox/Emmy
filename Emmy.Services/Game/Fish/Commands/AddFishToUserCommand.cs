using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Fish.Commands
{
    public record AddFishToUserCommand(long UserId, Guid FishId, uint Amount) : IRequest;

    public class AddFishToUserHandler : IRequestHandler<AddFishToUserCommand>
    {
        private readonly ILogger<AddFishToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddFishToUserHandler(
            DbContextOptions options,
            ILogger<AddFishToUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(AddFishToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserFishes
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.FishId == request.FishId);

            if (entity is null)
            {
                var created = await _db.CreateEntity(new UserFish
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    FishId = request.FishId,
                    Amount = request.Amount,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user fish entity {@Entity}",
                    created);
            }
            else
            {
                entity.Amount += request.Amount;
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Added user {UserId} fish {FishId} amount {Amount}",
                    request.UserId, request.FishId, request.Amount);
            }

            return Unit.Value;
        }
    }
}