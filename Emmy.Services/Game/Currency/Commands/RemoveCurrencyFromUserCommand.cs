using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Currency.Commands
{
    public record RemoveCurrencyFromUserCommand(
            long UserId,
            Emmy.Data.Enums.Currency Type,
            uint Amount)
        : IRequest;

    public class RemoveCurrencyFromUserHandler : IRequestHandler<RemoveCurrencyFromUserCommand>
    {
        private readonly ILogger<RemoveCurrencyFromUserHandler> _logger;
        private readonly AppDbContext _db;

        public RemoveCurrencyFromUserHandler(
            DbContextOptions options,
            ILogger<RemoveCurrencyFromUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(RemoveCurrencyFromUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserCurrencies
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                throw new Exception($"user {request.UserId} doesnt have currency {request.Type.ToString()}");
            }

            entity.Amount -= request.Amount;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Removed from user {UserId} currency {Currency} amount {Amount}",
                request.UserId, request.Type.ToString(), request.Amount);

            return Unit.Value;
        }
    }
}