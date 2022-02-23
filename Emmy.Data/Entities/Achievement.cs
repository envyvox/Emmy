using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class Achievement : INamedEntity, ICreatedEntity
    {
        public Enums.Achievement Type { get; set; }
        public AchievementCategory Category { get; set; }
        public string Name { get; set; }
        public AchievementRewardType RewardType { get; set; }
        public uint RewardNumber { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.HasKey(x => x.Type);
            builder.HasIndex(x => x.Type).IsUnique();

            builder.Property(x => x.Category).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.RewardType).IsRequired();
            builder.Property(x => x.RewardNumber).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}