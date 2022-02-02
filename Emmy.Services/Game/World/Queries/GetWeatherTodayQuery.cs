using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.World.Queries
{
    public record GetWeatherTodayQuery : IRequest<Weather>;

    public class GetWeatherTodayHandler : IRequestHandler<GetWeatherTodayQuery, Weather>
    {
        private readonly AppDbContext _db;

        public GetWeatherTodayHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<Weather> Handle(GetWeatherTodayQuery request, CancellationToken ct)
        {
            return await _db.WorldStates
                .AsQueryable()
                .Select(x => x.WeatherToday)
                .SingleOrDefaultAsync();
        }
    }
}