using System;
using Emmy.Data;
using Emmy.Services.Discord.Client;
using Emmy.Services.Discord.Extensions;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Localization.Impl;
using Emmy.Services.Hangfire.BackgroundJobs.ActivityReward;
using Emmy.Services.Hangfire.BackgroundJobs.CompleteFarmWatering;
using Emmy.Services.Hangfire.BackgroundJobs.CompleteFishing;
using Emmy.Services.Hangfire.BackgroundJobs.CompleteTransit;
using Emmy.Services.Hangfire.BackgroundJobs.DeleteDailyRewards;
using Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredLoveRooms;
using Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredPremiums;
using Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredPrivateRooms;
using Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredLoveRooms;
using Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredPremiums;
using Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredPrivateRooms;
using Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredRoles;
using Emmy.Services.Hangfire.BackgroundJobs.StartNewDay;
using Emmy.Services.Hangfire.BackgroundJobs.VoiceStatistic;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Emmy
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DiscordClientOptions>(x => _config.GetSection("DiscordOptions").Bind(x));

            services.AddDbContextPool<AppDbContext>(o =>
            {
                o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                o.UseNpgsql(_config.GetConnectionString("main"),
                    s => { s.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name); });
            });

            services.AddHangfireServer();
            services.AddHangfire(config =>
            {
                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute {Attempts = 0});
                config.UsePostgreSqlStorage(_config.GetConnectionString("main"));
            });

            services.AddAutoMapper(typeof(IDiscordClientService).Assembly);
            services.AddMediatR(typeof(IDiscordClientService).Assembly);

            services
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                );

            services.AddOpenApiDocument();

            services.AddSingleton(_ =>
                TimeZoneInfo.FindSystemTimeZoneById(_config.GetValue<string>("CronTimezoneId")));

            services.AddSingleton<CommandHandler>();
            services.AddSingleton<IDiscordClientService, DiscordClientService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();

            services.AddScoped<IVoiceStatisticJob, VoiceStatisticJob>();
            services.AddScoped<IActivityRewardJob, ActivityRewardJob>();
            services.AddScoped<IRemoveExpiredRolesJob, RemoveExpiredRolesJob>();
            services.AddScoped<IRemoveExpiredLoveRoomsJob, RemoveExpiredLoveRoomsJob>();
            services.AddScoped<IRemoveExpiredPrivateRoomsJob, RemoveExpiredPrivateRoomsJob>();
            services.AddScoped<IRemoveExpiredPremiumsJob, RemoveExpiredPremiumsJob>();
            services.AddScoped<INotifyExpiredLoveRoomsJob, NotifyExpiredLoveRoomsJob>();
            services.AddScoped<INotifyExpiredPrivateRoomsJob, NotifyExpiredPrivateRoomsJob>();
            services.AddScoped<INotifyExpiredPremiumsJob, NotifyExpiredPremiumsJob>();
            services.AddScoped<ICompleteFishingJob, CompleteFishingJob>();
            services.AddScoped<IStartNewDayJob, StartNewDayJob>();
            services.AddScoped<IDeleteDailyRewardsJob, DeleteDailyRewardsJob>();
            services.AddScoped<ICompleteTransitJob, CompleteTransitJob>();
            services.AddScoped<ICompleteFarmWateringJob, CompleteFarmWateringJob>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            MigrateDb(app.ApplicationServices);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] {new AllowAllAuthorizationFilter()}
            });

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .Build());

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.StartDiscord();
        }

        private static void MigrateDb(IServiceProvider serviceProvider)
        {
            using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }

        private class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                return true;
            }
        }
    }
}