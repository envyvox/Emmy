using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Transit.Commands
{
    public record CreateTransitCommand(
            Location Departure,
            Location Destination,
            TimeSpan Duration,
            uint Price)
        : IRequest;

    public class CreateTransitHandler : IRequestHandler<CreateTransitCommand>
    {
        private readonly ILogger<CreateTransitHandler> _logger;
        private readonly AppDbContext _db;

        public CreateTransitHandler(
            DbContextOptions options,
            ILogger<CreateTransitHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(CreateTransitCommand request, CancellationToken ct)
        {
            var exist = await _db.Transits
                .AnyAsync(x =>
                    x.Departure == request.Departure &&
                    x.Destination == request.Destination);

            if (exist)
            {
                throw new Exception(
                    $"transit from {request.Departure.ToString()} to {request.Destination.ToString()} already exist");
            }

            var created = await _db.CreateEntity(new Emmy.Data.Entities.Transit
            {
                Id = Guid.NewGuid(),
                Departure = request.Departure,
                Destination = request.Destination,
                Duration = request.Duration,
                Price = request.Price
            });

            _logger.LogInformation(
                "Created transit entity {@Entity}",
                created);

            return Unit.Value;
        }
    }
}