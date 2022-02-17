using System;
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
    public record ResetUserFarmCommand(long UserId, uint Number) : IRequest;

    public class ResetUserFarmHandler : IRequestHandler<ResetUserFarmCommand>
    {
        private readonly ILogger<ResetUserFarmHandler> _logger;
        private readonly AppDbContext _db;

        public ResetUserFarmHandler(
            DbContextOptions options,
            ILogger<ResetUserFarmHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(ResetUserFarmCommand request, CancellationToken ct)
        {
            var entity = await _db.UserFarms
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Number == request.Number);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have farm {request.Number}");
            }

            entity.State = FieldState.Empty;
            entity.SeedId = null;
            entity.Progress = 0;
            entity.InReGrowth = false;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Reseted user {UserId} farm {Number} to default values",
                request.UserId, request.Number);

            return Unit.Value;
        }
    }
}