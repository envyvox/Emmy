using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.DailyReward.Commands
{
    public record CreateUserDailyRewardCommand(long UserId, DayOfWeek DayOfWeek) : IRequest;

    public class CreateUserDailyRewardHandler : IRequestHandler<CreateUserDailyRewardCommand>
    {
        private readonly ILogger<CreateUserDailyRewardHandler> _logger;
        private readonly AppDbContext _db;

        public CreateUserDailyRewardHandler(
            DbContextOptions options,
            ILogger<CreateUserDailyRewardHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateUserDailyRewardCommand request, CancellationToken ct)
        {
            var exist = await _db.UserDailyRewards
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.DayOfWeek == request.DayOfWeek);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} daily reward for day {request.DayOfWeek.ToString()} already exist");
            }

            var created = await _db.CreateEntity(new UserDailyReward
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                DayOfWeek = request.DayOfWeek
            });

            _logger.LogInformation(
                "Created user daily reward entity {@Entity}",
                created);

            return Unit.Value;
        }
    }
}