using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Guild.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Role.Commands
{
    public record DeleteUserRoleCommand(long UserId, long RoleId) : IRequest;

    public class DeleteUserRoleHandler : IRequestHandler<DeleteUserRoleCommand>
    {
        private readonly ILogger<DeleteUserRoleHandler> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public DeleteUserRoleHandler(
            DbContextOptions options,
            ILogger<DeleteUserRoleHandler> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DeleteUserRoleCommand request, CancellationToken ct)
        {
            var entity = await _db.UserRoles
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.RoleId == request.RoleId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have role {request.RoleId} entity");
            }

            await _db.DeleteEntity(entity);

            _logger.LogInformation(
                "Deleted user role entity {@Entity}",
                entity);

            var hasRole = await _mediator.Send(new CheckGuildUserHasRoleByIdQuery(
                (ulong) request.UserId, (ulong) request.RoleId));

            if (hasRole)
            {
                await _mediator.Send(new RemoveRoleFromGuildUserByRoleIdCommand(
                    (ulong) request.UserId, (ulong) request.RoleId));
            }

            return Unit.Value;
        }
    }
}