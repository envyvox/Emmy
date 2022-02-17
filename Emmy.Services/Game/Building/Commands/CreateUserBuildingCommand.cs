using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Building.Commands
{
    public record CreateUserBuildingCommand(long UserId, Data.Enums.Building Building) : IRequest;

    public class CreateUserBuildingHandler : IRequestHandler<CreateUserBuildingCommand>
    {
        private readonly ILogger<CreateUserBuildingHandler> _logger;
        private readonly AppDbContext _db;

        public CreateUserBuildingHandler(
            DbContextOptions options,
            ILogger<CreateUserBuildingHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateUserBuildingCommand request, CancellationToken ct)
        {
            var exist = await _db.UserBuildings
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Building == request.Building);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already have building {request.Building.ToString()}");
            }

            var created = await _db.CreateEntity(new UserBuilding
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Building = request.Building,
                CreatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created user building entity {@Entity}",
                created);

            return Unit.Value;
        }
    }
}