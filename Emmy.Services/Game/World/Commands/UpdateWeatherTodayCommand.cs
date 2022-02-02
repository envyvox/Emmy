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
    public record UpdateWeatherTodayCommand(Weather WeatherToday) : IRequest;

    public class UpdateWeatherTodayHandler : IRequestHandler<UpdateWeatherTodayCommand>
    {
        private readonly ILogger<UpdateWeatherTodayHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateWeatherTodayHandler(
            DbContextOptions options,
            ILogger<UpdateWeatherTodayHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(UpdateWeatherTodayCommand request, CancellationToken ct)
        {
            var entity = await _db.WorldStates.SingleOrDefaultAsync();

            entity.WeatherToday = request.WeatherToday;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated weather today to {Weather}",
                request.WeatherToday.ToString());

            return Unit.Value;
        }
    }
}