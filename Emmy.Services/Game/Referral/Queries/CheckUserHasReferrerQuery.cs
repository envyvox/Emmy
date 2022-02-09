using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Referral.Queries
{
    public record CheckUserHasReferrerQuery(long UserId) : IRequest<bool>;

    public class CheckUserHasReferrerHandler : IRequestHandler<CheckUserHasReferrerQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckUserHasReferrerHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckUserHasReferrerQuery request, CancellationToken ct)
        {
            return await _db.UserReferrers
                .AnyAsync(x => x.UserId == request.UserId);
        }
    }
}