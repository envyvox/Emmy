using System;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Services.Game.Farm.Commands;
using Emmy.Services.Game.World.Commands;
using Emmy.Services.Game.World.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.BackgroundJobs.StartNewDay
{
    public class StartNewDayJob : IStartNewDayJob
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StartNewDayJob> _logger;

        private readonly Random _random = new();

        public StartNewDayJob(
            IMediator mediator,
            ILogger<StartNewDayJob> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Start new day job executed");

            await GenerateWeather();
            await _mediator.Send(new MoveAllFarmsProgressCommand());
            await _mediator.Send(new UpdateAllFarmsStateCommand(FieldState.Planted));
        }

        private async Task GenerateWeather()
        {
            _logger.LogInformation(
                "Generate weather executed");

            var chance = _random.Next(1, 101);
            var oldWeatherToday = await _mediator.Send(new GetWeatherTodayQuery());
            var newWeatherToday = await _mediator.Send(new GetWeatherTomorrowQuery());

            var newWeatherTomorrow = newWeatherToday == Weather.Clear
                ? chance + (oldWeatherToday == Weather.Rain ? 10 : 20) > 50
                    ? Weather.Rain
                    : Weather.Clear
                : chance + (oldWeatherToday == Weather.Clear ? 10 : 20) > 50
                    ? Weather.Clear
                    : Weather.Rain;

            await _mediator.Send(new UpdateWeatherTodayCommand(newWeatherToday));
            await _mediator.Send(new UpdateWeatherTomorrowCommand(newWeatherTomorrow));
        }
    }
}