using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Transit.Commands
{
    public record DeleteUserMovementCommand(long UserId) : IRequest;

    public class DeleteUserMovementHandler : IRequestHandler<DeleteUserMovementCommand>
    {
        private readonly ILogger<DeleteUserMovementHandler> _logger;
        private readonly AppDbContext _db;

        public DeleteUserMovementHandler(
            DbContextOptions options,
            ILogger<DeleteUserMovementHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(DeleteUserMovementCommand request, CancellationToken ct)
        {
            var entity = await _db.UserMovements
                .SingleOrDefaultAsync(x => x.UserId == request.UserId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have movement entity");
            }

            await _db.DeleteEntity(entity);

            _logger.LogInformation(
                "Deleted user movement entity for user {UserId}",
                request.UserId);

            return Unit.Value;
        }
    }
}