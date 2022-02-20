using System;
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
    public record GetFishQuery(Guid Id) : IRequest<FishDto>;

    public class GetFishHandler : IRequestHandler<GetFishQuery, FishDto>
    {
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _db;

        public GetFishHandler(
            DbContextOptions options,
            IMapper mapper,
            IMemoryCache cache)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<FishDto> Handle(GetFishQuery request, CancellationToken ct)
        {
            if (_cache.TryGetValue(string.Format(CacheExtensions.FishKey, request.Id), out FishDto fish)) return fish;

            var entity = await _db.Fishes
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (entity is null)
            {
                throw new Exception(
                    $"fish {request.Id} not found");
            }

            fish = _mapper.Map<FishDto>(entity);

            _cache.Set(string.Format(CacheExtensions.FishKey, request.Id), fish, CacheExtensions.DefaultCacheOptions);

            return fish;
        }
    }
}