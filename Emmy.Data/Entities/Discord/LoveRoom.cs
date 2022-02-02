using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.Discord
{
    public class LoveRoom : IUniqueIdentifiedEntity, IExpirationEntity, ICreatedEntity, IUpdatedEntity
    {
        public Guid Id { get; set; }
        public Guid RelationshipId { get; set; }
        public long ChannelId { get; set; }
        public DateTimeOffset Expiration { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public Relationship Relationship { get; set; }
    }

    public class LoveRoomConfiguration : IEntityTypeConfiguration<LoveRoom>
    {
        public void Configure(EntityTypeBuilder<LoveRoom> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.RelationshipId).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.ChannelId).IsRequired();
            builder.Property(x => x.Expiration).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder
                .HasOne(x => x.Relationship)
                .WithOne()
                .HasForeignKey<LoveRoom>(x => x.RelationshipId);
        }
    }
}