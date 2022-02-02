using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.Discord
{
    public class ContentVote : IUniqueIdentifiedEntity, ICreatedEntity, IUpdatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public Guid ContentMessageId { get; set; }
        public Vote Vote { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public User.User User { get; set; }
        public ContentMessage ContentMessage { get; set; }
    }

    public class ContentVoteConfiguration : IEntityTypeConfiguration<ContentVote>
    {
        public void Configure(EntityTypeBuilder<ContentVote> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.UserId, x.ContentMessageId, x.Vote});

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Vote).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.ContentMessage)
                .WithMany()
                .HasForeignKey(x => x.ContentMessageId);
        }
    }
}