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

namespace Emmy.Services.Game.Key.Commands
{
    public record AddKeyToUserCommand(long UserId, KeyType Type, uint Amount = 1) : IRequest;

    public class AddKeyToUserHandler : IRequestHandler<AddKeyToUserCommand>
    {
        private readonly ILogger<AddKeyToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddKeyToUserHandler(
            DbContextOptions options,
            ILogger<AddKeyToUserHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(AddKeyToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserKeys
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                await _db.CreateEntity(new UserKey
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Type = request.Type,
                    Amount = request.Amount,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });
            }
            else
            {
                entity.Amount += request.Amount;
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);
            }

            _logger.LogInformation(
                "Added user {UserId} key {Type} amount {Amount}",
                request.UserId, request.Type.ToString(), request.Amount);

            return Unit.Value;
        }
    }
}