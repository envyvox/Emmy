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
    public record AddXpToUserCommand(long UserId, uint Amount) : IRequest;

    public class AddXpToUserHandler : IRequestHandler<AddXpToUserCommand>
    {
        private readonly ILogger<AddXpToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddXpToUserHandler(
            DbContextOptions options,
            ILogger<AddXpToUserHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(AddXpToUserCommand request, CancellationToken ct)
        {
            var entity = await _db.Users
                .SingleOrDefaultAsync(x => x.Id == request.UserId);

            entity.Xp += request.Amount;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Added xp to user {UserId} amount {Amount}",
                request.UserId, request.Amount);
            
            // todo check level up

            return Unit.Value;
        }
    }
}