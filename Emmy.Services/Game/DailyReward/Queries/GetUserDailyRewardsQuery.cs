using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.DailyReward.Queries
{
    public record GetUserDailyRewardsQuery(long UserId) : IRequest<List<DayOfWeek>>;

    public class GetUserDailyRewardsHandler : IRequestHandler<GetUserDailyRewardsQuery, List<DayOfWeek>>
    {
        private readonly AppDbContext _db;

        public GetUserDailyRewardsHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<List<DayOfWeek>> Handle(GetUserDailyRewardsQuery request, CancellationToken ct)
        {
            var entities = await _db.UserDailyRewards
                .AsQueryable()
                .Where(x => x.UserId == request.UserId)
                .Select(x => x.DayOfWeek)
                .ToListAsync();

            return entities;
        }
    }
}