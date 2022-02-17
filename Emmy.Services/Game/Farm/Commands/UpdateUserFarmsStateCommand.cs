using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Farm.Commands
{
    public record UpdateUserFarmsStateCommand(long UserId, FieldState State) : IRequest;

    public class UpdateUserFarmsStateHandler : IRequestHandler<UpdateUserFarmsStateCommand>
    {
        private readonly ILogger<UpdateUserFarmsStateHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateUserFarmsStateHandler(
            DbContextOptions options,
            ILogger<UpdateUserFarmsStateHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserFarmsStateCommand request, CancellationToken ct)
        {
            var entities = await _db.UserFarms
                .AsQueryable()
                .Where(x =>
                    x.UserId == request.UserId &&
                    x.State == FieldState.Planted)
                .ToListAsync();

            foreach (var field in entities)
            {
                field.State = request.State;
                field.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(field);

                _logger.LogInformation(
                    "Updated user {UserId} farm {Number} to state {State}",
                    request.UserId, field.Number, request.State);
            }

            return Unit.Value;
        }
    }
}