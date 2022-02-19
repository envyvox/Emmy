using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Services.Game.World.Commands;
using Emmy.Services.Game.World.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.BackgroundJobs.ChangeSeason
{
    public class ChangeSeasonJob : IChangeSeasonJob
    {
        private readonly ILogger<ChangeSeasonJob> _logger;
        private readonly IMediator _mediator;

        public ChangeSeasonJob(
            ILogger<ChangeSeasonJob> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Change season job executed");

            var currentSeason = await _mediator.Send(new GetCurrentSeasonQuery());

            await _mediator.Send(new UpdateCurrentSeasonCommand(currentSeason.NextSeason()));
        }
    }
}