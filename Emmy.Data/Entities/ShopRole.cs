using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class ShopRole : IPricedEntity, ICreatedEntity
    {
        public long RoleId { get; set; }
        public uint Price { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class ShopRoleConfiguration : IEntityTypeConfiguration<ShopRole>
    {
        public void Configure(EntityTypeBuilder<ShopRole> builder)
        {
            builder.HasKey(x => x.RoleId);
            builder.HasIndex(x => x.RoleId).IsUnique();

            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}