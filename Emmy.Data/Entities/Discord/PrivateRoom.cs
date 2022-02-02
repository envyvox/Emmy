using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.Discord
{
    public class PrivateRoom : IUniqueIdentifiedEntity, IExpirationEntity, ICreatedEntity, IUpdatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long ChannelId { get; set; }
        public DateTimeOffset Expiration { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public User.User User { get; set; }
    }

    public class PrivateRoomConfiguration : IEntityTypeConfiguration<PrivateRoom>
    {
        public void Configure(EntityTypeBuilder<PrivateRoom> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.ChannelId).IsRequired();
            builder.Property(x => x.Expiration).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
        }
    }
}