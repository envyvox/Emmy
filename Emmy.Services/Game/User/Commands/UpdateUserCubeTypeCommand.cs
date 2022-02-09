using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.User.Commands
{
    public record UpdateUserCubeTypeCommand(long UserId, CubeType CubeType) : IRequest;

    public class UpdateUserCubeTypeHandler : IRequestHandler<UpdateUserCubeTypeCommand>
    {
        private readonly ILogger<UpdateUserCubeTypeHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateUserCubeTypeHandler(
            DbContextOptions options,
            ILogger<UpdateUserCubeTypeHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserCubeTypeCommand request, CancellationToken ct)
        {
            var entity = await _db.Users
                .SingleOrDefaultAsync(x => x.Id == request.UserId);

            entity.CubeType = request.CubeType;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated user {UserId} cube type to {CubeType}",
                request.UserId, request.CubeType);

            return Unit.Value;
        }
    }
}