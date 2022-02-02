using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.World.Commands
{
    public record UpdateWeatherTomorrowCommand(Weather WeatherTomorrow) : IRequest;

    public class UpdateWeatherTomorrowHandler : IRequestHandler<UpdateWeatherTomorrowCommand>
    {
        private readonly ILogger<UpdateWeatherTomorrowHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateWeatherTomorrowHandler(
            DbContextOptions options,
            ILogger<UpdateWeatherTomorrowHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(UpdateWeatherTomorrowCommand request, CancellationToken ct)
        {
            var entity = await _db.WorldStates.SingleOrDefaultAsync();

            entity.WeatherTomorrow = request.WeatherTomorrow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated weather tomorrow to {Weather}",
                request.WeatherTomorrow.ToString());

            return Unit.Value;
        }
    }
}