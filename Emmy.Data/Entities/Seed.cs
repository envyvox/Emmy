using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class Seed : IUniqueIdentifiedEntity, INamedEntity, IPricedEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Season Season { get; set; }
        public uint GrowthDays { get; set; }
        public uint ReGrowthDays { get; set; }
        public bool IsMultiply { get; set; }
        public uint Price { get; set; }

        public Crop Crop { get; set; }
    }

    public class SeedConfiguration : IEntityTypeConfiguration<Seed>
    {
        public void Configure(EntityTypeBuilder<Seed> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Season).IsRequired();
            builder.Property(x => x.GrowthDays).IsRequired();
            builder.Property(x => x.ReGrowthDays).IsRequired();
            builder.Property(x => x.IsMultiply).IsRequired();
            builder.Property(x => x.Price).IsRequired();
        }
    }
}