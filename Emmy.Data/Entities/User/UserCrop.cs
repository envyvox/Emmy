using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.User
{
    public class UserCrop : IUniqueIdentifiedEntity, IAmountEntity, ICreatedEntity, IUpdatedEntity
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public Guid CropId { get; set; }
        public uint Amount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public User User { get; set; }
        public Crop Crop { get; set; }
    }

    public class UserCropConfiguration : IEntityTypeConfiguration<UserCrop>
    {
        public void Configure(EntityTypeBuilder<UserCrop> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new {x.UserId, x.CropId}).IsUnique();

            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.Crop)
                .WithMany()
                .HasForeignKey(x => x.CropId);
        }
    }
}