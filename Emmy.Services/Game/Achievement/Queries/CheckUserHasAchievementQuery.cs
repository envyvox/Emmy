using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Achievement.Queries
{
    public record CheckUserHasAchievementQuery(long UserId, Data.Enums.Achievement Type) : IRequest<bool>;

    public class CheckUserHasAchievementHandler : IRequestHandler<CheckUserHasAchievementQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckUserHasAchievementHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckUserHasAchievementQuery request, CancellationToken ct)
        {
            return await _db.UserAchievements
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);
        }
    }
}