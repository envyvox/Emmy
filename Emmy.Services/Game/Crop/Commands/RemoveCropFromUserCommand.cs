using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Crop.Commands
{
    public record RemoveCropFromUserCommand(long UserId, Guid CropId, uint Amount) : IRequest;

    public class RemoveCropFromUserHandler : IRequestHandler<RemoveCropFromUserCommand>
    {
        private readonly ILogger<RemoveCropFromUserHandler> _logger;
        private readonly AppDbContext _db;

        public RemoveCropFromUserHandler(
            DbContextOptions options,
            ILogger<RemoveCropFromUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(RemoveCropFromUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserCrops
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.CropId == request.CropId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have entity with crop {request.CropId}");
            }

            entity.Amount -= request.Amount;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Removed from user {UserId} crop {CropId} amount {Amount}",
                request.UserId, request.CropId, request.Amount);

            return Unit.Value;
        }
    }
}