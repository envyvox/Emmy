using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.LoveRoom.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Relationship.Commands
{
    public record DeleteRelationshipCommand(long UserId) : IRequest;

    public class DeleteRelationshipHandler : IRequestHandler<DeleteRelationshipCommand>
    {
        private readonly ILogger<DeleteRelationshipHandler> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public DeleteRelationshipHandler(
            DbContextOptions options,
            ILogger<DeleteRelationshipHandler> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DeleteRelationshipCommand request, CancellationToken ct)
        {
            var entity = await _db.Relationships
                .SingleOrDefaultAsync(x =>
                    x.User1Id == request.UserId ||
                    x.User2Id == request.UserId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have relationship");
            }

            var hasLoveRoom = await _mediator.Send(new CheckRelationshipHasLoveRoomQuery(entity.Id));

            if (hasLoveRoom)
            {
                var loveRoom = await _mediator.Send(new GetLoveRoomQuery(entity.Id));
                var guild = await _mediator.Send(new GetSocketGuildQuery());

                await guild.GetChannel((ulong) loveRoom.ChannelId).DeleteAsync();
            }

            await _db.DeleteEntity(entity);

            _logger.LogInformation(
                "Deleted relationship entity {@Entity}",
                entity);

            return Unit.Value;
        }
    }
}