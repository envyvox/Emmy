using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Farm.Commands
{
    public record UpdateAllFarmsStateCommand(FieldState State) : IRequest;

    public class UpdateAllFarmsStateHandler : IRequestHandler<UpdateAllFarmsStateCommand>
    {
        private readonly ILogger<UpdateAllFarmsStateHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateAllFarmsStateHandler(
            DbContextOptions options,
            ILogger<UpdateAllFarmsStateHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateAllFarmsStateCommand request, CancellationToken ct)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
                update user_farms
                set state = {request.State},
                    updated_at = {DateTimeOffset.UtcNow}
                where state != {FieldState.Empty}
                  and state != {FieldState.Completed}");

            _logger.LogInformation(
                "Farms state updated to {State}",
                request.State.ToString());

            return Unit.Value;
        }
    }
}