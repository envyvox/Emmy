using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Seed.Commands
{
    public record RemoveSeedFromUserCommand(long UserId, Guid SeedId, uint Amount) : IRequest;

    public class RemoveSeedFromUserHandler : IRequestHandler<RemoveSeedFromUserCommand>
    {
        private readonly ILogger<RemoveSeedFromUserHandler> _logger;
        private readonly AppDbContext _db;

        public RemoveSeedFromUserHandler(
            DbContextOptions options,
            ILogger<RemoveSeedFromUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(RemoveSeedFromUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserSeeds
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.SeedId == request.SeedId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have seed {request.SeedId}");
            }

            entity.Amount -= request.Amount;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Removed from user {UserId} seed {SeedId} amount {Amount}",
                request.UserId, request.SeedId, request.Amount);

            return Unit.Value;
        }
    }
}