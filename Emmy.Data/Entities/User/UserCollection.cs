using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.User
{
    public class UserCollection : IUniqueIdentifiedEntity, ICreatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public CollectionCategory Category { get; set; }
        public Guid ItemId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; }
    }

    public class UserCollectionConfiguration : IEntityTypeConfiguration<UserCollection>
    {
        public void Configure(EntityTypeBuilder<UserCollection> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.UserId, Type = x.Category, x.ItemId}).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Category).IsRequired();
            builder.Property(x => x.ItemId).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
        }
    }
}