using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.User.Commands
{
    public record UpdateUserOnGuildCommand(long UserId, bool OnGuild) : IRequest;

    public class UpdateUserOnGuildHandler : IRequestHandler<UpdateUserOnGuildCommand>
    {
        private readonly ILogger<UpdateUserOnGuildHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateUserOnGuildHandler(
            DbContextOptions options,
            ILogger<UpdateUserOnGuildHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserOnGuildCommand request, CancellationToken ct)
        {
            var entity = await _db.Users
                .SingleOrDefaultAsync(x => x.Id == request.UserId);

            entity.OnGuild = request.OnGuild;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated user {UserId} on guild {OnGuild}",
                request.UserId, request.OnGuild);

            return Unit.Value;
        }
    }
}