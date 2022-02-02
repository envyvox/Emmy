using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.World.Queries
{
    public record GetWeatherTomorrowQuery : IRequest<Weather>;

    public class GetWeatherTomorrowHandler : IRequestHandler<GetWeatherTomorrowQuery, Weather>
    {
        private readonly AppDbContext _db;

        public GetWeatherTomorrowHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<Weather> Handle(GetWeatherTomorrowQuery request, CancellationToken ct)
        {
            return await _db.WorldStates
                .AsQueryable()
                .Select(x => x.WeatherTomorrow)
                .SingleOrDefaultAsync();
        }
    }
}