using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Building.Queries
{
    public record CheckUserHasBuildingQuery(long UserId, Data.Enums.Building Building) : IRequest<bool>;

    public class CheckUserHasBuildingHandler : IRequestHandler<CheckUserHasBuildingQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckUserHasBuildingHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckUserHasBuildingQuery request, CancellationToken ct)
        {
            return await _db.UserBuildings
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Building == request.Building);
        }
    }
}