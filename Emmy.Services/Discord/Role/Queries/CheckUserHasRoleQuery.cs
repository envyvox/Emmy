using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.Role.Queries
{
    public record CheckUserHasRoleQuery(long UserId, long RoleId) : IRequest<bool>;

    public class CheckUserHasRoleHandler : IRequestHandler<CheckUserHasRoleQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckUserHasRoleHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckUserHasRoleQuery request, CancellationToken ct)
        {
            return await _db.UserRoles
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.RoleId == request.RoleId);
        }
    }
}