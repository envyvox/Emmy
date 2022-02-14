using System;
using Emmy.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class Fraction : IUpdatedEntity
    {
        public Enums.Fraction Type { get; set; }
        public uint Points { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class FractionConfiguration : IEntityTypeConfiguration<Fraction>
    {
        public void Configure(EntityTypeBuilder<Fraction> builder)
        {
            builder.HasKey(x => x.Type);
            builder.HasIndex(x => x.Type).IsUnique();

            builder.Property(x => x.Points).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}