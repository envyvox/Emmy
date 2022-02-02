using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Currency.Commands
{
    public record AddCurrencyToUserCommand(
            long UserId,
            Emmy.Data.Enums.Currency Type,
            uint Amount)
        : IRequest;

    public class AddCurrencyToUserHandler : IRequestHandler<AddCurrencyToUserCommand>
    {
        private readonly ILogger<AddCurrencyToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddCurrencyToUserHandler(
            DbContextOptions options,
            ILogger<AddCurrencyToUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(AddCurrencyToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserCurrencies
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                await _db.CreateEntity(new UserCurrency
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Type = request.Type,
                    Amount = request.Amount,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user currency entity for user {UserId} with currency {Currency} and amount {Amount}",
                    request.UserId, request.Type.ToString(), request.Amount);
            }
            else
            {
                entity.Amount += request.Amount;
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Added user {UserId} currency {Currency} amount {Amount}",
                    request.UserId, request.Type.ToString(), request.Amount);
            }

            return Unit.Value;
        }
    }
}