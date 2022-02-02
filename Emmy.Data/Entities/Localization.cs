using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class Localization : IUniqueIdentifiedEntity
    {
        public Guid Id { get; set; }
        public LocalizationCategory Category { get; set; }
        public string Name { get; set; }
        public string Single { get; set; }
        public string Double { get; set; }
        public string Multiply { get; set; }

        public string Localize(uint amount)
        {
            var n = Math.Abs(amount);

            n %= 100;
            if (n is >= 5 and <= 20) return Multiply;

            n %= 10;
            if (n == 1) return Single;
            if (n is >= 2 and <= 4) return Double;

            return Multiply;
        }
    }

    public class LocalizationConfiguration : IEntityTypeConfiguration<Localization>
    {
        public void Configure(EntityTypeBuilder<Localization> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.Category, x.Name}).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Category).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Single).IsRequired();
            builder.Property(x => x.Double).IsRequired();
            builder.Property(x => x.Multiply).IsRequired();
        }
    }
}