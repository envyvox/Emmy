using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Key.Commands
{
    public record RemoveKeyFromUserCommand(long UserId, KeyType Type, uint Amount = 1) : IRequest;

    public class RemoveKeyFromUserHandler : IRequestHandler<RemoveKeyFromUserCommand>
    {
        private readonly ILogger<RemoveKeyFromUserHandler> _logger;
        private readonly AppDbContext _db;

        public RemoveKeyFromUserHandler(
            DbContextOptions options,
            ILogger<RemoveKeyFromUserHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveKeyFromUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserKeys
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have key {request.Type.ToString()}");
            }

            entity.Amount -= request.Amount;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Removed from user {UserId} key {Type} amount {Amount}",
                request.UserId, request.Type.ToString(), request.Amount);

            return Unit.Value;
        }
    }
}