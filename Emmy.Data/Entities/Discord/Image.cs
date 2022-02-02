using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities.Discord
{
    public class Image
    {
        public Enums.Image Type { get; set; }
        public string Url { get; set; }
    }

    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(x => x.Type);
            builder.HasIndex(x => x.Type);

            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Url).IsRequired();
        }
    }
}