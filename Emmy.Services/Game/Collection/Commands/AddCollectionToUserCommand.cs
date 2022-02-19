using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Collection.Commands
{
    public record AddCollectionToUserCommand(long UserId, CollectionCategory Category, Guid ItemId) : IRequest;

    public class AddCollectionToUserHandler : IRequestHandler<AddCollectionToUserCommand>
    {
        private readonly ILogger<AddCollectionToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddCollectionToUserHandler(
            DbContextOptions options,
            ILogger<AddCollectionToUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(AddCollectionToUserCommand request, CancellationToken ct)
        {
            var exist = await _db.UserCollections
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Category == request.Category &&
                    x.ItemId == request.ItemId);

            if (exist) return Unit.Value;

            var created = await _db.CreateEntity(new UserCollection
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Category = request.Category,
                ItemId = request.ItemId,
                CreatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created user collection entity {@Entity}",
                created);

            // todo check collection achievements

            return Unit.Value;
        }
    }
}