using System;
using System.Linq;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.BackgroundJobs.GenerateDynamicShopBanner
{
    public class GenerateDynamicShopBannerJob : IGenerateDynamicShopBannerJob
    {
        private readonly ILogger<GenerateDynamicShopBannerJob> _logger;
        private readonly AppDbContext _db;

        public GenerateDynamicShopBannerJob(
            DbContextOptions options,
            ILogger<GenerateDynamicShopBannerJob> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Generate dynamic shop banner job executed");

            await _db.Database.ExecuteSqlRawAsync("truncate dynamic_shop_banners;");

            var banners = await _db.Banners
                .OrderByRandom()
                .Where(x => 
                    x.Name != "Ночной город" && // ignore default banner
                    x.Rarity != BannerRarity.Limited &&
                    x.Rarity != BannerRarity.Custom) 
                .Take(5)
                .ToListAsync();

            foreach (var banner in banners)
            {
                await _db.CreateEntity(new DynamicShopBanner { Id = Guid.NewGuid(), BannerId = banner.Id });

                _logger.LogInformation(
                    "Created dynamic shop entity for banner {BannerId}",
                    banner.Id);
            }
        }
    }
}
