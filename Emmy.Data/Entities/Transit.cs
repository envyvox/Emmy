using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class Transit : IUniqueIdentifiedEntity, IPricedEntity
    {
        public Guid Id { get; set; }
        public Location Departure { get; set; }
        public Location Destination { get; set; }
        public TimeSpan Duration { get; set; }
        public uint Price { get; set; }
    }

    public class TransitConfiguration : IEntityTypeConfiguration<Transit>
    {
        public void Configure(EntityTypeBuilder<Transit> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.Departure, x.Destination}).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Departure).IsRequired();
            builder.Property(x => x.Destination).IsRequired();
            builder.Property(x => x.Duration).IsRequired();
            builder.Property(x => x.Price).IsRequired();
        }
    }
}