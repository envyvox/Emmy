using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.User
{
    public class UserKey : IUniqueIdentifiedEntity, IAmountEntity, ICreatedEntity, IUpdatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public KeyType Type { get; set; }
        public uint Amount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public User User { get; set; }
        public Key Key { get; set; }
        
    }

    public class UserKeyConfiguration : IEntityTypeConfiguration<UserKey>
    {
        public void Configure(EntityTypeBuilder<UserKey> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.UserId, x.Type}).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.Key)
                .WithMany()
                .HasForeignKey(x => x.Type);
        }
    }
}