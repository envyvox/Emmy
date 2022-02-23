using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Achievement.Commands
{
    public record CreateUserAchievementCommand(long UserId, Data.Enums.Achievement Type) : IRequest;

    public class CreateUserAchievementHandler : IRequestHandler<CreateUserAchievementCommand>
    {
        private readonly ILogger<CreateUserAchievementHandler> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public CreateUserAchievementHandler(
            DbContextOptions options,
            ILogger<CreateUserAchievementHandler> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateUserAchievementCommand request, CancellationToken ct)
        {
            var exist = await _db.UserAchievements
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already have achievement {request.Type.ToString()}");
            }

            var created = await _db.CreateEntity(new UserAchievement
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = request.Type,
                CreatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created user achievement entity {@Entity}",
                created);

            return await _mediator.Send(new AddAchievementRewardToUserCommand(request.UserId, request.Type));
        }
    }
}