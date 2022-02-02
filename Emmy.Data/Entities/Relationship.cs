using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class Relationship : IUniqueIdentifiedEntity, ICreatedEntity
    {
        public Guid Id { get; set; }
        public long User1Id { get; set; }
        public long User2Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User.User User1 { get; set; }
        public User.User User2 { get; set; }
    }

    public class UserRelationshipConfiguration : IEntityTypeConfiguration<Relationship>
    {
        public void Configure(EntityTypeBuilder<Relationship> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.User1Id).IsUnique();
            builder.HasIndex(x => x.User2Id).IsUnique();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder
                .HasOne(x => x.User1)
                .WithOne()
                .HasForeignKey<Relationship>(x => x.User1Id);

            builder
                .HasOne(x => x.User2)
                .WithOne()
                .HasForeignKey<Relationship>(x => x.User2Id);
        }
    }
}