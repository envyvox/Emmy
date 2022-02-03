using System;
using System.Collections.Generic;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class Fish : IUniqueIdentifiedEntity, INamedEntity, IPricedEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public FishRarity Rarity { get; set; }
        public Weather CatchWeather { get; set; }
        public TimesDayType CatchTimesDay { get; set; }
        public List<Season> CatchSeasons { get; set; }
        public uint Price { get; set; }
    }

    public class FishConfiguration : IEntityTypeConfiguration<Fish>
    {
        public void Configure(EntityTypeBuilder<Fish> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Rarity).IsRequired();
            builder.Property(x => x.CatchWeather).IsRequired();
            builder.Property(x => x.CatchTimesDay).IsRequired();
            builder.Property(x => x.CatchSeasons).IsRequired();
            builder.Property(x => x.Price).IsRequired();
        }
    }
}