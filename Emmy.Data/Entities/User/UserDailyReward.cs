using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.User
{
    public class UserDailyReward : IUniqueIdentifiedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public User User { get; set; }
    }

    public class UserDailyRewardConfiguration : IEntityTypeConfiguration<UserDailyReward>
    {
        public void Configure(EntityTypeBuilder<UserDailyReward> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.UserId, x.DayOfWeek}).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
        }
    }
}