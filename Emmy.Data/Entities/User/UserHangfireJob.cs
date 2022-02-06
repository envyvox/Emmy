using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.User
{
    public class UserHangfireJob : IUniqueIdentifiedEntity, ICreatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public HangfireJobType Type { get; set; }
        public string JobId { get; set; }
        public DateTimeOffset Expiration { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; }
    }

    public class UserHangfireJobConfiguration : IEntityTypeConfiguration<UserHangfireJob>
    {
        public void Configure(EntityTypeBuilder<UserHangfireJob> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.UserId, x.Type}).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.JobId).IsRequired();
            builder.Property(x => x.Expiration).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
        }
    }
}