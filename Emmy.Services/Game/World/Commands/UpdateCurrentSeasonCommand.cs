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
    public record UpdateCurrentSeasonCommand(Season Season) : IRequest;

    public class UpdateCurrentSeasonHandler : IRequestHandler<UpdateCurrentSeasonCommand>
    {
        private readonly ILogger<UpdateCurrentSeasonHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateCurrentSeasonHandler(
            DbContextOptions options,
            ILogger<UpdateCurrentSeasonHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(UpdateCurrentSeasonCommand request, CancellationToken ct)
        {
            var entity = await _db.WorldStates.SingleOrDefaultAsync();

            entity.CurrentSeason = request.Season;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated current season to {Season}",
                request.Season.ToString());

            return Unit.Value;
        }
    }
}