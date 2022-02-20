using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Fish.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CacheExtensions = Emmy.Services.Extensions.CacheExtensions;

namespace Emmy.Services.Game.Fish.Queries
{
    public record GetFishesQuery : IRequest<List<FishDto>>;

    public class GetFishesHandler : IRequestHandler<GetFishesQuery, List<FishDto>>
    {
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _db;

        public GetFishesHandler(
            DbContextOptions options,
            IMapper mapper,
            IMemoryCache cache)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<FishDto>> Handle(GetFishesQuery request, CancellationToken ct)
        {
            if (_cache.TryGetValue(CacheExtensions.FishesKey, out List<FishDto> fishes)) return fishes;

            var entities = await _db.Fishes
                .AsQueryable()
                .OrderBy(x => x.Name)
                .ToListAsync();

            fishes = _mapper.Map<List<FishDto>>(entities);

            _cache.Set(CacheExtensions.FishesKey, fishes, CacheExtensions.DefaultCacheOptions);

            return fishes;
        }
    }
}