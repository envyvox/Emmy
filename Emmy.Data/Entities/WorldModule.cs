using Emmy.Data.Enums.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emmy.Data.Entities
{
    public class WorldModule
    {
        public CommandModule Module { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class WorldModuleConfiguration : IEntityTypeConfiguration<WorldModule>
    {
        public void Configure(EntityTypeBuilder<WorldModule> builder)
        {
            builder.HasKey(x => x.Module);
            builder.HasIndex(x => x.Module).IsUnique();

            builder.Property(x => x.Module).IsRequired();
            builder.Property(x => x.IsEnabled).IsRequired();
        }
    }
}