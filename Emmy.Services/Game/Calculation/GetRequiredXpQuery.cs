using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using CacheExtensions = Emmy.Services.Extensions.CacheExtensions;

namespace Emmy.Services.Game.Calculation
{
    public record GetRequiredXpQuery(uint Level) : IRequest<uint>;

    public class GetRequiredXpHandler : IRequestHandler<GetRequiredXpQuery, uint>
    {
        private readonly IMemoryCache _cache;

        public GetRequiredXpHandler(IMemoryCache cache)
        {
            _cache = cache;
        }

        private const uint FixedIncrease = 500;

        public async Task<uint> Handle(GetRequiredXpQuery request, CancellationToken ct)
        {
            if (_cache.TryGetValue(string.Format(CacheExtensions.XpRequired, request.Level), out uint requiredXp))
                return await Task.FromResult(requiredXp);

            requiredXp = CalculateXpRequired(request.Level);

            _cache.Set(string.Format(CacheExtensions.XpRequired, request.Level), requiredXp,
                CacheExtensions.DefaultCacheOptions);

            return await Task.FromResult(requiredXp);
        }

        private static uint CalculateXpRequired(uint level)
        {
            uint leap = level switch
            {
                >= 1 and <= 20 => 35,
                >= 21 and <= 40 => 33,
                >= 41 and <= 60 => 60,
                >= 61 and <= 80 => 110,
                >= 81 and <= 100 => 255,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };

            return level > 2
                ? CalculateXpRequired(level - 1) + FixedIncrease + leap * (level - 2)
                : FixedIncrease;
        }
    }
}