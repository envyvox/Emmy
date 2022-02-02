using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.Discord
{
    public class ContentMessage : IUniqueIdentifiedEntity, ICreatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long ChannelId { get; set; }
        public long MessageId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User.User User { get; set; }
    }

    public class ContentMessageConfiguration : IEntityTypeConfiguration<ContentMessage>
    {
        public void Configure(EntityTypeBuilder<ContentMessage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.UserId, x.ChannelId, x.MessageId});

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.ChannelId).IsRequired();
            builder.Property(x => x.MessageId).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
        }
    }
}