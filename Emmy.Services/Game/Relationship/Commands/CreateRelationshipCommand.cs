using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Relationship.Commands
{
    public record CreateRelationshipCommand(long User1Id, long User2Id) : IRequest;

    public class CreateRelationshipHandler : IRequestHandler<CreateRelationshipCommand>
    {
        private readonly ILogger<CreateRelationshipHandler> _logger;
        private readonly AppDbContext _db;

        public CreateRelationshipHandler(
            DbContextOptions options,
            ILogger<CreateRelationshipHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateRelationshipCommand request, CancellationToken ct)
        {
            var exist = await _db.Relationships
                .AnyAsync(x =>
                    x.User1Id == request.User1Id ||
                    x.User1Id == request.User2Id ||
                    x.User2Id == request.User1Id ||
                    x.User2Id == request.User2Id);

            if (exist)
            {
                throw new Exception(
                    $"one of users {request.User1Id} {request.User2Id} already have relationship");
            }

            var created = await _db.CreateEntity(new Data.Entities.Relationship
            {
                Id = Guid.NewGuid(),
                User1Id = request.User1Id,
                User2Id = request.User2Id,
                CreatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created relationship entity {@Entity}",
                created);

            return Unit.Value;
        }
    }
}