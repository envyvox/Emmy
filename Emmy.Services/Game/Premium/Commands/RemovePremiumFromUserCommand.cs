using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Enums.Discord;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Role.Commands;
using Emmy.Services.Discord.Role.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Premium.Commands
{
    public record RemovePremiumFromUserCommand(long UserId) : IRequest;

    public class RemovePremiumFromUserHandler : IRequestHandler<RemovePremiumFromUserCommand>
    {
        private readonly ILogger<RemovePremiumFromUserHandler> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public RemovePremiumFromUserHandler(
            DbContextOptions options,
            ILogger<RemovePremiumFromUserHandler> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(RemovePremiumFromUserCommand request, CancellationToken ct)
        {
            var entity = await _db.UserPremiums
                .SingleOrDefaultAsync(x => x.UserId == request.UserId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have premium entity");
            }

            await _db.DeleteEntity(entity);

            _logger.LogInformation(
                "Deleted user premium entity for user {UserId}",
                request.UserId);

            var roles = DiscordRepository.Roles;

            await _mediator.Send(new UpdateUserPremiumCommand(request.UserId, false));
            await _mediator.Send(new DeleteUserRoleCommand(request.UserId, (long) roles[Role.Premium].Id));

            var hasPersonalRole = await _mediator.Send(new CheckUserHasPersonalRoleQuery(request.UserId));

            if (hasPersonalRole)
            {
                var personalRole = await _mediator.Send(new GetUserPersonalRoleQuery(request.UserId));

                await _mediator.Send(new DeleteSocketRoleFromGuildCommand((ulong) personalRole.RoleId));
            }

            var hasBoost = await _mediator.Send(new CheckGuildUserHasRoleByTypeQuery(
                (ulong) request.UserId, Role.NitroBoost));

            await _mediator.Send(new UpdateUserCubeTypeCommand(
                request.UserId, hasBoost ? CubeType.D8 : CubeType.D6));

            return Unit.Value;
        }
    }
}