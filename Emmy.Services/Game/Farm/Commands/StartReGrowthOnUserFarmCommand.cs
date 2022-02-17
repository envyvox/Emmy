using System;
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
    public record StartReGrowthOnUserFarmCommand(long UserId, uint Number) : IRequest;

    public class StartReGrowthOnUserFarmHandler : IRequestHandler<StartReGrowthOnUserFarmCommand>
    {
        private readonly ILogger<StartReGrowthOnUserFarmHandler> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public StartReGrowthOnUserFarmHandler(
            DbContextOptions options,
            ILogger<StartReGrowthOnUserFarmHandler> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(StartReGrowthOnUserFarmCommand request, CancellationToken ct)
        {
            var entity = await _db.UserFarms
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Number == request.Number);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have farm with number {request.Number}");
            }

            var weather = await _mediator.Send(new GetWeatherTodayQuery());

            entity.Progress = 0;
            entity.InReGrowth = true;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            entity.State = weather == Weather.Clear
                ? FieldState.Planted
                : FieldState.Watered;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Started re growth on user {UserId} farm {Number}",
                request.UserId, request.Number);

            return Unit.Value;
        }
    }
}