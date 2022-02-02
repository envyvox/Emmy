using System;
using System.Linq;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredRoles
{
    public class RemoveExpiredRolesJob : IRemoveExpiredRolesJob
    {
        private readonly ILogger<RemoveExpiredRolesJob> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public RemoveExpiredRolesJob(
            ILogger<RemoveExpiredRolesJob> logger,
            IMediator mediator,
            DbContextOptions options)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Remove expired roles job executed");

            var timeNow = DateTimeOffset.UtcNow;
            var expiredUserRoles = await _db.UserRoles
                .AsQueryable()
                .Where(x => x.Expiration < timeNow)
                .ToListAsync();

            foreach (var expiredUserRole in expiredUserRoles)
            {
                await _db.DeleteEntity(expiredUserRole);

                _logger.LogInformation(
                    "Deleted user role entity {@UserRole}",
                    expiredUserRole);

                var guild = await _mediator.Send(new GetSocketGuildQuery());
                var guildUser = guild.GetUser((ulong) expiredUserRole.UserId);
                var hasRole = guildUser.Roles.Any(x => x.Id == (ulong) expiredUserRole.RoleId);

                if (hasRole)
                {
                    await guildUser.RemoveRoleAsync((ulong) expiredUserRole.RoleId);

                    _logger.LogInformation(
                        "Removed role {RoleId} from user {UserId}",
                        expiredUserRole.RoleId, expiredUserRole.UserId);
                }

                if (expiredUserRole.IsPersonal)
                {
                    var role = guild.GetRole((ulong) expiredUserRole.RoleId);

                    await role.DeleteAsync();

                    _logger.LogInformation(
                        "Deleted role {@Role} from guild",
                        role);
                }
            }
        }
    }
}