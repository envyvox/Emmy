using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Container.Commands
{
    public record AddContainerToUserCommand(long UserId, Data.Enums.Container Type, uint Amount) : IRequest;

    public class AddContainerToUserHandler : IRequestHandler<AddContainerToUserCommand>
    {
        private readonly ILogger<AddContainerToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddContainerToUserHandler(
            DbContextOptions options,
            ILogger<AddContainerToUserHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(AddContainerToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserContainers
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                var created = await _db.CreateEntity(new UserContainer
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Type = request.Type,
                    Amount = request.Amount,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user container entity {@Entity}",
                    created);
            }
            else
            {
                entity.Amount += request.Amount;
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Added user {UserId} container {Type} amount {Amount}",
                    request.UserId, request.Type.ToString(), request.Amount);
            }

            return Unit.Value;
        }
    }
}