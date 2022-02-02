using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Banner.Queries
{
    public record CheckUserHasBannerQuery(long UserId, Guid BannerId) : IRequest<bool>;

    public class CheckUserHasBannerHandler : IRequestHandler<CheckUserHasBannerQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckUserHasBannerHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckUserHasBannerQuery request, CancellationToken ct)
        {
            return await _db.UserBanners
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.BannerId == request.BannerId);
        }
    }
}