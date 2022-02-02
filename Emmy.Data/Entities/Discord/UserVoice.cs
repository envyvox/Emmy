using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.Discord
{
    public class UserVoice : IUniqueIdentifiedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long ChannelId { get; set; }

        public User.User User { get; set; }
    }

    public class UserVoiceConfiguration : IEntityTypeConfiguration<UserVoice>
    {
        public void Configure(EntityTypeBuilder<UserVoice> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.ChannelId).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<UserVoice>(x => x.UserId);
        }
    }
}