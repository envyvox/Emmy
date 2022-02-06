using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Game.Seed.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Seed.Queries
{
    public record GetSeedsBySeasonQuery(Season Season) : IRequest<List<SeedDto>>;

    public class GetSeedsBySeasonHandler : IRequestHandler<GetSeedsBySeasonQuery, List<SeedDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetSeedsBySeasonHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<SeedDto>> Handle(GetSeedsBySeasonQuery request, CancellationToken cancellationToken)
        {
            var entities = await _db.Seeds
                .Include(x => x.Crop)
                .OrderBy(x => x.Name)
                .Where(x => x.Season == request.Season)
                .ToListAsync();

            return _mapper.Map<List<SeedDto>>(entities);
        }
    }
}