using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Donate.Commands
{
    public record AddDonateToUserCommand(long UserId, uint Amount) : IRequest;

    public class AddDonateToUserHandler : IRequestHandler<AddDonateToUserCommand>
    {
        private readonly ILogger<AddDonateToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddDonateToUserHandler(
            DbContextOptions options,
            ILogger<AddDonateToUserHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(AddDonateToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserDonations
                .SingleOrDefaultAsync(x => x.UserId == request.UserId);

            if (entity is null)
            {
                var created = await _db.CreateEntity(new UserDonation
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Amount = request.Amount,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user donation entity {@Entity}",
                    created);
            }
            else
            {
                entity.Amount += request.Amount;
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Updated user donation entity for user {UserId} added {Amount}",
                    request.UserId, request.Amount);
            }

            return Unit.Value;
        }
    }
}