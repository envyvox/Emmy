using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.Role.Queries
{
    public record CheckUserHasPersonalRoleQuery(long UserId) : IRequest<bool>;

    public class CheckUserHasPersonalRoleHandler : IRequestHandler<CheckUserHasPersonalRoleQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckUserHasPersonalRoleHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckUserHasPersonalRoleQuery request, CancellationToken ct)
        {
            return await _db.UserRoles
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.IsPersonal);
        }
    }
}