using System;
using Emmy.Data;
using Emmy.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Localization.Impl
{
    public class LocalizationService : ILocalizationService
    {
        private readonly AppDbContext _db;

        public LocalizationService(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public string Localize(LocalizationCategory category, string keyword, uint amount = 1)
        {
            var entity = _db.Localizations
                .SingleOrDefaultAsync(x =>
                    x.Category == category &&
                    x.Name == keyword)
                .Result;

            if (entity is null)
            {
                throw new Exception(
                    $"localization with category {category.ToString()} and keyword {keyword} not found");
            }

            return entity.Localize(amount);
        }
    }
}