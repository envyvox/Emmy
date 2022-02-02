using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Game.Relationship.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.LoveRoom.Commands
{
    public record CreateLoveRoomCommand(RelationshipDto Relationship, TimeSpan Duration) : IRequest<ulong>;

    public class CreateLoveRoomHandler : IRequestHandler<CreateLoveRoomCommand, ulong>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateLoveRoomHandler> _logger;
        private readonly AppDbContext _db;

        public CreateLoveRoomHandler(
            DbContextOptions options,
            IMediator mediator,
            ILogger<CreateLoveRoomHandler> logger)
        {
            _db = new AppDbContext(options);
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ulong> Handle(CreateLoveRoomCommand request, CancellationToken ct)
        {
            var exist = await _db.LoveRooms
                .AnyAsync(x => x.RelationshipId == request.Relationship.Id);

            if (exist)
            {
                throw new Exception(
                    $"relationship {request.Relationship.Id} already have love room");
            }

            var channelId = await _mediator.Send(new CreateGuildLoveRoomCommand(
                (ulong) request.Relationship.User1.Id, (ulong) request.Relationship.User2.Id));

            var created = await _db.CreateEntity(new Data.Entities.Discord.LoveRoom
            {
                Id = Guid.NewGuid(),
                RelationshipId = request.Relationship.Id,
                ChannelId = (long) channelId,
                Expiration = DateTimeOffset.UtcNow.Add(request.Duration),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created love room entity {@Entity}",
                created);

            return channelId;
        }
    }
}