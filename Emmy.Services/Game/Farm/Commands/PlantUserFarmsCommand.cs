using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using Emmy.Services.Game.World.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Farm.Commands
{
    public record PlantUserFarmsCommand(
            long UserId,
            uint[] Numbers,
            Guid SeedId)
        : IRequest;

    public class PlantUserFarmsHandler : IRequestHandler<PlantUserFarmsCommand>
    {
        private readonly ILogger<PlantUserFarmsHandler> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public PlantUserFarmsHandler(
            DbContextOptions options,
            ILogger<PlantUserFarmsHandler> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(PlantUserFarmsCommand request, CancellationToken ct)
        {
            var entities = await _db.UserFarms
                .AsQueryable()
                .Where(x =>
                    x.UserId == request.UserId &&
                    request.Numbers.Contains(x.Number))
                .ToListAsync();

            if (entities.Any() is false)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have farms with numbers {request.Numbers}");
            }

            var weather = await _mediator.Send(new GetWeatherTodayQuery());
            var state = weather == Weather.Clear
                ? FieldState.Planted
                : FieldState.Watered;

            foreach (var entity in entities)
            {
                entity.UpdatedAt = DateTimeOffset.UtcNow;
                entity.SeedId = request.SeedId;
                entity.State = state;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Planted user {UserId} farm {Number} with seed {SeedId} and state {State}",
                    request.UserId, entity.Number, request.SeedId, state.ToString());
            }

            return Unit.Value;
        }
    }
}