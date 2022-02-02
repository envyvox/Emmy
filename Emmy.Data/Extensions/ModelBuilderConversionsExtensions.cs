using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Emmy.Data.Extensions
{
    public static class ModelBuilderConversionsExtensions
    {
        public static ModelBuilder UseValueConverterForType<T>(
            this ModelBuilder modelBuilder, ValueConverter converter, Action<ExceptBuilder> excepts = null)
        {
            return modelBuilder.UseValueConverterForType(typeof(T), converter, excepts);
        }

        public static ModelBuilder UseValueConverterForType(
            this ModelBuilder modelBuilder, Type type, ValueConverter converter, Action<ExceptBuilder> excepts = null)
        {
            var exceptBuilder = new ExceptBuilder();
            excepts?.Invoke(exceptBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType
                    .GetProperties()
                    .Where(p => p.PropertyType == type);

                foreach (var property in properties)
                {
                    if (exceptBuilder.IsExcepted(entityType.ClrType, property))
                    {
                        continue;
                    }

                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(converter);
                }
            }

            return modelBuilder;
        }
    }
}