using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Farm.Commands
{
    public record CheckCompletedFarmsCommand : IRequest;

    public class CheckCompletedFarmsHandler : IRequestHandler<CheckCompletedFarmsCommand>
    {
        private readonly ILogger<CheckCompletedFarmsHandler> _logger;
        private readonly AppDbContext _db;

        public CheckCompletedFarmsHandler(
            DbContextOptions options,
            ILogger<CheckCompletedFarmsHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(CheckCompletedFarmsCommand request, CancellationToken ct)
        {
            var entities = await _db.UserFarms
                .AsQueryable()
                .Where(x =>
                    x.State != FieldState.Empty &&
                    x.State != FieldState.Completed &&
                    (x.InReGrowth == false && x.Progress >= x.Seed.GrowthDays ||
                     x.InReGrowth == true && x.Progress >= x.Seed.ReGrowthDays))
                .ToListAsync();

            foreach (var entity in entities)
            {
                entity.State = FieldState.Completed;
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "User {UserId} farm {Number} state marked as completed",
                    entity.UserId, entity.Number);
            }

            return Unit.Value;
        }
    }
}