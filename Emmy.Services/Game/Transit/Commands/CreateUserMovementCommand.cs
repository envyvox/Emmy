using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Transit.Commands
{
    public record CreateUserMovementCommand(
            long UserId,
            Location Departure,
            Location Destination,
            TimeSpan Duration)
        : IRequest;

    public class CreateUserMovementHandler : IRequestHandler<CreateUserMovementCommand>
    {
        private readonly ILogger<CreateUserMovementHandler> _logger;
        private readonly AppDbContext _db;

        public CreateUserMovementHandler(
            DbContextOptions options,
            ILogger<CreateUserMovementHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(CreateUserMovementCommand request, CancellationToken ct)
        {
            var exist = await _db.UserMovements
                .AnyAsync(x => x.UserId == request.UserId);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already have movement entity");
            }

            var created = await _db.CreateEntity(new UserMovement
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Departure = request.Departure,
                Destination = request.Destination,
                Arrival = DateTimeOffset.UtcNow.Add(request.Duration),
                CreatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created user movement entity {@Entity}",
                created);

            return Unit.Value;
        }
    }
}