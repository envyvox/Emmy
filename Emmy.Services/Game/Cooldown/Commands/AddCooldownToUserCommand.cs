using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Cooldown.Commands
{
    public record AddCooldownToUserCommand(long UserId, Data.Enums.Cooldown Type, TimeSpan Duration) : IRequest;

    public class AddCooldownToUserHandler : IRequestHandler<AddCooldownToUserCommand>
    {
        private readonly ILogger<AddCooldownToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddCooldownToUserHandler(
            DbContextOptions options,
            ILogger<AddCooldownToUserHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(AddCooldownToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserCooldowns
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                var created = await _db.CreateEntity(new UserCooldown
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Type = request.Type,
                    Expiration = DateTimeOffset.UtcNow.Add(request.Duration),
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user cooldown entity {@Entity}",
                    created);
            }
            else
            {
                entity.Expiration = DateTimeOffset.UtcNow.Add(request.Duration);
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Added user {UserId} cooldown {Type} duration {Duration}",
                    request.UserId, request.Type.ToString(), request.Duration);
            }

            return Unit.Value;
        }
    }
}