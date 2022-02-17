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
    public record MoveAllFarmsProgressCommand : IRequest;

    public class MoveAllFarmsProgressHandler : IRequestHandler<MoveAllFarmsProgressCommand>
    {
        private readonly ILogger<MoveAllFarmsProgressHandler> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public MoveAllFarmsProgressHandler(
            DbContextOptions options,
            ILogger<MoveAllFarmsProgressHandler> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(MoveAllFarmsProgressCommand request, CancellationToken ct)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
                update user_farms
                set progress = progress + 1,
                    updated_at = {DateTimeOffset.UtcNow}
                where state = {FieldState.Watered}");

            _logger.LogInformation(
                "Farms progress updated to +1");

            return await _mediator.Send(new CheckCompletedFarmsCommand());
        }
    }
}