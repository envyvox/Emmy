using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.World.Queries
{
    public record GetCurrentSeasonQuery : IRequest<Season>;

    public class GetCurrentSeasonHandler : IRequestHandler<GetCurrentSeasonQuery, Season>
    {
        private readonly AppDbContext _db;

        public GetCurrentSeasonHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<Season> Handle(GetCurrentSeasonQuery request, CancellationToken ct)
        {
            return await _db.WorldStates
                .AsQueryable()
                .Select(x => x.CurrentSeason)
                .SingleOrDefaultAsync();
        }
    }
}