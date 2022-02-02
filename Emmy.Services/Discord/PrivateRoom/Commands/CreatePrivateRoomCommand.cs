using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Guild.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.PrivateRoom.Commands
{
    public record CreatePrivateRoomCommand(long OwnerId, TimeSpan Duration) : IRequest<ulong>;

    public class CreatePrivateRoomHandler : IRequestHandler<CreatePrivateRoomCommand, ulong>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreatePrivateRoomHandler> _logger;
        private readonly AppDbContext _db;

        public CreatePrivateRoomHandler(
            DbContextOptions options,
            IMediator mediator,
            ILogger<CreatePrivateRoomHandler> logger)
        {
            _db = new AppDbContext(options);
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ulong> Handle(CreatePrivateRoomCommand request, CancellationToken ct)
        {
            var createdChannelId = await _mediator.Send(new CreateGuildPrivateRoomCommand((ulong) request.OwnerId));

            var created = await _db.CreateEntity(new Data.Entities.Discord.PrivateRoom
            {
                Id = Guid.NewGuid(),
                UserId = request.OwnerId,
                ChannelId = (long) createdChannelId,
                Expiration = DateTimeOffset.UtcNow.Add(request.Duration),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created private room entity {@Created}",
                created);

            return createdChannelId;
        }
    }
}