using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.LoveRoom.Commands
{
    public record UpdateLoveRoomCommand(Guid RelationshipId, TimeSpan Duration) : IRequest;

    public class UpdateLoveRoomHandler : IRequestHandler<UpdateLoveRoomCommand>
    {
        private readonly ILogger<UpdateLoveRoomHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateLoveRoomHandler(
            DbContextOptions options,
            ILogger<UpdateLoveRoomHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateLoveRoomCommand request, CancellationToken ct)
        {
            var entity = await _db.LoveRooms
                .SingleOrDefaultAsync(x => x.RelationshipId == request.RelationshipId);

            if (entity is null)
            {
                throw new Exception(
                    $"relationship {request.RelationshipId} doesnt have love room");
            }

            entity.Expiration = entity.Expiration.Add(request.Duration);
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated relationship {RelationshipId} love room added {Duration}",
                request.RelationshipId, request.Duration);

            return Unit.Value;
        }
    }
}