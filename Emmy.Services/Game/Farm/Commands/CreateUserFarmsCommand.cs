using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Farm.Commands
{
    public record CreateUserFarmsCommand(long UserId, IEnumerable<uint> Numbers) : IRequest;

    public class CreateUserFarmsHandler : IRequestHandler<CreateUserFarmsCommand>
    {
        private readonly ILogger<CreateUserFarmsHandler> _logger;
        private readonly AppDbContext _db;

        public CreateUserFarmsHandler(
            DbContextOptions options,
            ILogger<CreateUserFarmsHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateUserFarmsCommand request, CancellationToken ct)
        {
            foreach (var number in request.Numbers)
            {
                var exist = await _db.UserFarms
                    .AnyAsync(x =>
                        x.UserId == request.UserId &&
                        x.Number == number);

                if (exist)
                {
                    throw new Exception(
                        $"user {request.UserId} already have farm with number {number}");
                }

                var created = await _db.CreateEntity(new UserFarm
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Number = number,
                    State = FieldState.Empty,
                    SeedId = null,
                    Progress = 0,
                    InReGrowth = false,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user farm entity {@Entity}",
                    created);
            }

            return Unit.Value;
        }
    }
}