using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Emmy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables("Emmy_")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Destructure.ToMaximumDepth(3)
                .Destructure.With<IgnoreNullablePropertiesDestructuringPolicy>()
                .CreateLogger();

            try
            {
                Log.Information("Application starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "The application failed to start correctly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureAppConfiguration(x => x.AddEnvironmentVariables("Emmy_"));
        }

        private class IgnoreNullablePropertiesDestructuringPolicy : IDestructuringPolicy
        {
            public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory,
                out LogEventPropertyValue result)
            {
                var properties = value
                    .GetType().GetTypeInfo().DeclaredProperties
                    .Where(x => x.GetValue(value) is not null)
                    .Select(x =>
                        new LogEventProperty(x.Name, propertyValueFactory.CreatePropertyValue(x.GetValue(value))));

                result = new StructureValue(properties);

                return true;
            }
        }
    }
}