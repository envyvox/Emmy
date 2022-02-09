using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.User
{
    public class UserReferrer : IUniqueIdentifiedEntity, ICreatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long ReferrerId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; }
        public User Referrer { get; set; }
    }

    public class UserReferrerConfiguration : IEntityTypeConfiguration<UserReferrer>
    {
        public void Configure(EntityTypeBuilder<UserReferrer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.UserId, x.ReferrerId}).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.Referrer)
                .WithMany()
                .HasForeignKey(x => x.ReferrerId);
        }
    }
}