using System;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.Discord
{
    public class UserGenderRequest : IUniqueIdentifiedEntity, ICreatedEntity, IUpdatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public RequestState State { get; set; }
        public long? ModeratorId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public User.User User { get; set; }
        public User.User Moderator { get; set; }
    }

    public class UserGenderRequestConfiguration : IEntityTypeConfiguration<UserGenderRequest>
    {
        public void Configure(EntityTypeBuilder<UserGenderRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.State).IsRequired();
            builder.Property(x => x.ModeratorId);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<UserGenderRequest>(x => x.UserId);

            builder
                .HasOne(x => x.Moderator)
                .WithMany()
                .HasForeignKey(x => x.ModeratorId);
        }
    }
}