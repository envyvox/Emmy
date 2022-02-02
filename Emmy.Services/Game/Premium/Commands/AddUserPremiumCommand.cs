using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums.Discord;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Role.Commands;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Premium.Commands
{
    public record AddUserPremiumCommand(long UserId, TimeSpan Duration) : IRequest;

    public class AddUserPremiumHandler : IRequestHandler<AddUserPremiumCommand>
    {
        private readonly ILogger<AddUserPremiumHandler> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public AddUserPremiumHandler(
            DbContextOptions options,
            ILogger<AddUserPremiumHandler> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(AddUserPremiumCommand request, CancellationToken ct)
        {
            var entity = await _db.UserPremiums
                .SingleOrDefaultAsync(x => x.UserId == request.UserId);

            if (entity is null)
            {
                var roles = DiscordRepository.Roles;

                var created = await _db.CreateEntity(new UserPremium
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Expiration = DateTimeOffset.UtcNow.Add(request.Duration),
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });

                _logger.LogInformation(
                    "Created user premium entity {@Entity}",
                    created);

                await _mediator.Send(new UpdateUserPremiumCommand(request.UserId, true));
                await _mediator.Send(new AddRoleToUserCommand(request.UserId, (long) roles[Role.Premium].Id, null));
            }
            else
            {
                entity.Expiration = entity.Expiration.Add(request.Duration);
                entity.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(entity);

                _logger.LogInformation(
                    "Updated user premium entity for user {UserId} added {Duration}",
                    request.UserId, request.Duration);
            }

            return Unit.Value;
        }
    }
}