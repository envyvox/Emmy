using System;
using Microsoft.Extensions.Caching.Memory;

namespace Emmy.Services.Extensions
{
    public static class CacheExtensions
    {
        public const string FishesKey = "fishes";
        public const string FishesWithSeasonKey = "fishes_season_{0}";
        public const string FishKey = "fish_{0}";

        public static readonly MemoryCacheEntryOptions DefaultCacheOptions =
            new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
    }
}