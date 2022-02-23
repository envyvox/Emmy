using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Statistic.Commands
{
    public record AddStatisticToUserCommand(long UserId, Data.Enums.Statistic Type, uint Amount = 1) : IRequest;

    public class AddStatisticToUserHandler : IRequestHandler<AddStatisticToUserCommand>
    {
        private readonly ILogger<AddStatisticToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddStatisticToUserHandler(
            DbContextOptions options,
            ILogger<AddStatisticToUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(AddStatisticToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserStatistics
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                await _db.CreateEntity(new UserStatistic
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Type = request.Type,
                    Amount = request.Amount,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user statistic entity for user {UserId} with type {Type} and amount {Amount}",
                    request.UserId, request.Type.ToString(), request.Amount);
            }
            else
            {
                entity.Amount += request.Amount;
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Added user {UserId} statistic {Type} amount {Amount}",
                    request.UserId, request.Type.ToString(), request.Amount);
            }

            return await AddAllTimeStatisticToUser(request.UserId, request.Type, request.Amount);
        }

        private async Task<Unit> AddAllTimeStatisticToUser(long userId, Data.Enums.Statistic type, uint amount = 1)
        {
            var entity = await _db.UserAllTimeStatistics
                .SingleOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Type == type);

            if (entity is null)
            {
                await _db.CreateEntity(new UserStatistic
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = type,
                    Amount = amount,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });
            }
            else
            {
                entity.Amount += amount;
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);
            }

            return Unit.Value;
        }
    }
}