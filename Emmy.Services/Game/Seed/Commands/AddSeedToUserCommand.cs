using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Seed.Commands
{
    public record AddSeedToUserCommand(long UserId, Guid SeedId, uint Amount) : IRequest;

    public class AddSeedToUserHandler : IRequestHandler<AddSeedToUserCommand>
    {
        private readonly ILogger<AddSeedToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddSeedToUserHandler(
            DbContextOptions options,
            ILogger<AddSeedToUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(AddSeedToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserSeeds
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.SeedId == request.SeedId);

            if (entity is null)
            {
                var created = await _db.CreateEntity(new UserSeed
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    SeedId = request.SeedId,
                    Amount = request.Amount,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user seed entity {@Entity}",
                    created);
            }
            else
            {
                entity.Amount += request.Amount;
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Added user {UserId} seed {SeedId} amount {Amount}",
                    request.UserId, request.SeedId, request.Amount);
            }

            return Unit.Value;
        }
    }
}