using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.User
{
    public class UserMovement : IUniqueIdentifiedEntity, ICreatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public Location Departure { get; set; }
        public Location Destination { get; set; }
        public DateTimeOffset Arrival { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; }
    }

    public class UserMovementsConfiguration : IEntityTypeConfiguration<UserMovement>
    {
        public void Configure(EntityTypeBuilder<UserMovement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Departure).IsRequired();
            builder.Property(x => x.Destination).IsRequired();
            builder.Property(x => x.Arrival).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
        }
    }
}