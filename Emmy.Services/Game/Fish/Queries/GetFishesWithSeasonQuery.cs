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
using Microsoft.Extensions.Caching.Memory;
using CacheExtensions = Emmy.Services.Extensions.CacheExtensions;

namespace Emmy.Services.Game.Fish.Queries
{
    public record GetFishesWithSeasonQuery(Season Season) : IRequest<List<FishDto>>;

    public class GetFishesWithSeasonHandler : IRequestHandler<GetFishesWithSeasonQuery, List<FishDto>>
    {
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _db;

        public GetFishesWithSeasonHandler(
            DbContextOptions options,
            IMapper mapper,
            IMemoryCache cache)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<FishDto>> Handle(GetFishesWithSeasonQuery request, CancellationToken ct)
        {
            if (_cache.TryGetValue(string.Format(CacheExtensions.FishesWithSeasonKey, request.Season),
                out List<FishDto> fishes)) return fishes;

            var entities = await _db.Fishes
                .AsQueryable()
                .ToListAsync();

            var filteredEntities = entities
                .Where(x =>
                    x.CatchSeasons.Contains(Season.Any) ||
                    x.CatchSeasons.Contains(request.Season))
                .ToList();

            fishes = _mapper.Map<List<FishDto>>(filteredEntities);

            _cache.Set(string.Format(CacheExtensions.FishesWithSeasonKey, request.Season), fishes,
                CacheExtensions.DefaultCacheOptions);

            return fishes;
        }
    }
}