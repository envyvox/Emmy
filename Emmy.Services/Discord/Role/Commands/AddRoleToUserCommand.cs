using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.Discord;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Role.Commands
{
    public record AddRoleToUserCommand(
            long UserId,
            long RoleId,
            TimeSpan? Duration,
            bool IsPersonal = false)
        : IRequest;

    public class AddRoleToUserHandler : IRequestHandler<AddRoleToUserCommand>
    {
        private readonly ILogger<AddRoleToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddRoleToUserHandler(
            DbContextOptions options,
            ILogger<AddRoleToUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(AddRoleToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserRoles
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.RoleId == request.RoleId);

            if (entity is null)
            {
                var created = await _db.CreateEntity(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    RoleId = request.RoleId,
                    IsPersonal = request.IsPersonal,
                    Expiration = request.Duration is null
                        ? null
                        : DateTimeOffset.UtcNow.Add((TimeSpan) request.Duration),
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user role entity {@UserRole}",
                    created);
            }
            else
            {
                entity.Expiration = request.Duration is null
                    ? null
                    : DateTimeOffset.UtcNow.Add((TimeSpan) request.Duration);
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Updated user role entity {@Entity}",
                    entity);
            }

            return Unit.Value;
        }
    }
}