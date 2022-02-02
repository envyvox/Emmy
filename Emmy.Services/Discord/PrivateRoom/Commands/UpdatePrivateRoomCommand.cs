using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.PrivateRoom.Commands
{
    public record UpdatePrivateRoomCommand(long UserId, long ChannelId, TimeSpan Duration) : IRequest;

    public class UpdatePrivateRoomHandler : IRequestHandler<UpdatePrivateRoomCommand>
    {
        private readonly ILogger<UpdatePrivateRoomHandler> _logger;
        private readonly AppDbContext _db;

        public UpdatePrivateRoomHandler(
            DbContextOptions options,
            ILogger<UpdatePrivateRoomHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdatePrivateRoomCommand request, CancellationToken ct)
        {
            var entity = await _db.PrivateRooms
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.ChannelId == request.ChannelId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have private room {request.ChannelId}");
            }

            entity.Expiration = entity.Expiration.Add(request.Duration);
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated user {UserId} private room {ChannelId} added {Duration}",
                request.UserId, request.ChannelId, request.Duration);

            return Unit.Value;
        }
    }
}