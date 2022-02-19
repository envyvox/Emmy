using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.User.Commands
{
    public record UpdateUserFractionCommand(long UserId, Emmy.Data.Enums.Fraction Fraction) : IRequest;

    public class UpdateUserFractionHandler : IRequestHandler<UpdateUserFractionCommand>
    {
        private readonly ILogger<UpdateUserFractionHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateUserFractionHandler(
            DbContextOptions options,
            ILogger<UpdateUserFractionHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserFractionCommand request, CancellationToken ct)
        {
            var entity = await _db.Users
                .SingleOrDefaultAsync(x => x.Id == request.UserId);

            entity.Fraction = request.Fraction;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated user {UserId} fraction to {Fraction}",
                request.UserId, request.Fraction.ToString());

            return Unit.Value;
        }
    }
}