using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Game.Fish.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Fish.Queries
{
    public record GetFishesWithSeasonQuery(Season Season) : IRequest<List<FishDto>>;

    public class GetFishesWithSeasonHandler : IRequestHandler<GetFishesWithSeasonQuery, List<FishDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetFishesWithSeasonHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<FishDto>> Handle(GetFishesWithSeasonQuery request, CancellationToken ct)
        {
            var entities = await _db.Fishes
                .AsQueryable()
                .ToListAsync();

            var filteredEntities = entities
                .Where(x =>
                    x.CatchSeasons.Contains(Season.Any) ||
                    x.CatchSeasons.Contains(request.Season))
                .ToList();

            return _mapper.Map<List<FishDto>>(filteredEntities);
        }
    }
}