using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Container.Commands
{
    public record RemoveContainerFromUserCommand(long UserId, Data.Enums.Container Type, uint Amount) : IRequest;

    public class RemoveContainerFromUserHandler : IRequestHandler<RemoveContainerFromUserCommand>
    {
        private readonly ILogger<RemoveContainerFromUserHandler> _logger;
        private readonly AppDbContext _db;

        public RemoveContainerFromUserHandler(
            DbContextOptions options,
            ILogger<RemoveContainerFromUserHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveContainerFromUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserContainers
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have container {request.Type.ToString()} entity");
            }

            entity.Amount -= request.Amount;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Removed from user {UserId} container {Type} amount {Amount}",
                request.UserId, request.Type.ToString(), request.Amount);

            return Unit.Value;
        }
    }
}