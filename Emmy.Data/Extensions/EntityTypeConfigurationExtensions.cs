using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Data.Extensions
{
    public static class EntityTypeConfigurationExtensions
    {
        private static readonly MethodInfo ApplyConfigurationMethod = typeof(EntityTypeConfigurationExtensions)
            .GetMethod("ApplyConfiguration", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly Dictionary<Assembly, IEnumerable<Type>> TypesPerAssembly = new();

        public static ModelBuilder UseEntityTypeConfiguration<T>(this ModelBuilder modelBuilder)
            where T : DbContext
        {
            var assembly = Assembly.GetAssembly(typeof(T));

            if (TypesPerAssembly.TryGetValue(assembly, out var configurationTypes) == false)
            {
                TypesPerAssembly[assembly] = configurationTypes = assembly
                    .GetExportedTypes()
                    .Where(x => x.GetTypeInfo().IsClass
                                && !x.GetTypeInfo().IsAbstract
                                && x.GetInterfaces().Any(y => y.GetTypeInfo().IsGenericType
                                                              && y.GetGenericTypeDefinition() ==
                                                              typeof(IEntityTypeConfiguration<>)));
            }

            var configurations = configurationTypes.Select(Activator.CreateInstance);

            foreach (var configuration in configurations)
            {
                var entityType = FindEntityType(configuration.GetType());

                var apply = ApplyConfigurationMethod.MakeGenericMethod(entityType);
                apply.Invoke(null, new[] {modelBuilder, configuration});
            }

            return modelBuilder;
        }

        private static Type FindEntityType(Type type)
        {
            var interfaceType = type.GetInterfaces()
                .First(x => x.GetTypeInfo().IsGenericType
                            && x.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));

            return interfaceType.GetGenericArguments().First();
        }

        private static void ApplyConfiguration<T>(ModelBuilder modelBuilder, IEntityTypeConfiguration<T> configuration)
            where T : class
        {
            configuration.Configure(modelBuilder.Entity<T>());
        }
    }
}