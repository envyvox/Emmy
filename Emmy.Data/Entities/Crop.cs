using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class Crop : IUniqueIdentifiedEntity, INamedEntity, IPricedEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public uint Price { get; set; }

        public Guid SeedId { get; set; }
        public Seed Seed { get; set; }
    }

    public class CropConfiguration : IEntityTypeConfiguration<Crop>
    {
        public void Configure(EntityTypeBuilder<Crop> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Price).IsRequired();

            builder
                .HasOne(x => x.Seed)
                .WithOne(x => x.Crop)
                .HasForeignKey<Crop>(x => x.SeedId);
        }
    }
}