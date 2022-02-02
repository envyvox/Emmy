using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class DynamicShopBanner : IUniqueIdentifiedEntity
    {
        public Guid Id { get; set; }
        public Guid BannerId { get; set; }

        public Banner Banner { get; set; }
    }

    public class DynamicShopBannerConfiguration : IEntityTypeConfiguration<DynamicShopBanner>
    {
        public void Configure(EntityTypeBuilder<DynamicShopBanner> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.BannerId).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();

            builder
                .HasOne(x => x.Banner)
                .WithOne()
                .HasForeignKey<DynamicShopBanner>(x => x.BannerId);
        }
    }
}