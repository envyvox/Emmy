﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Interactions;
using Emmy.Services.Discord.Emote.Commands;
using Emmy.Services.Discord.Extensions;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Hangfire.BackgroundJobs.ActivityReward;
using Emmy.Services.Hangfire.BackgroundJobs.GenerateDynamicShopBanner;
using Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredLoveRooms;
using Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredPremiums;
using Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredPrivateRooms;
using Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredLoveRooms;
using Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredPremiums;
using Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredPrivateRooms;
using Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredRoles;
using Emmy.Services.Hangfire.BackgroundJobs.VoiceStatistic;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emmy.Services.Discord.Client.Events
{
    public record OnReady(InteractionService InteractionService) : IRequest;

    public class OnReadyHandler : IRequestHandler<OnReady>
    {
        private readonly IWebHostEnvironment _env;
        private readonly DiscordClientOptions _options;
        private readonly ILogger<OnReadyHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly TimeZoneInfo _timeZoneInfo;

        public OnReadyHandler(
            IWebHostEnvironment env,
            IOptions<DiscordClientOptions> options,
            ILogger<OnReadyHandler> logger,
            IMediator mediator,
            IHostApplicationLifetime lifetime,
            TimeZoneInfo timeZoneInfo)
        {
            _env = env;
            _options = options.Value;
            _logger = logger;
            _mediator = mediator;
            _lifetime = lifetime;
            _timeZoneInfo = timeZoneInfo;
        }

        public async Task<Unit> Handle(OnReady request, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new SyncChannelsCommand());
                await _mediator.Send(new SyncRolesCommand());
                await _mediator.Send(new SyncEmotesCommand());

                RecurringJob.AddOrUpdate<IVoiceStatisticJob>("voice-statistic",
                    x => x.Execute(),
                    Cron.Minutely, _timeZoneInfo);

                RecurringJob.AddOrUpdate<IGenerateDynamicShopBannerJob>("generate-dynamic-shop-banners",
                    x => x.Execute(),
                    Cron.Daily, _timeZoneInfo);
                RecurringJob.AddOrUpdate<IRemoveExpiredRolesJob>("remove-expired-roles",
                    x => x.Execute(),
                    Cron.Daily, _timeZoneInfo);
                RecurringJob.AddOrUpdate<IRemoveExpiredLoveRoomsJob>("remove-expired-love-rooms",
                    x => x.Execute(),
                    Cron.Daily, _timeZoneInfo);
                RecurringJob.AddOrUpdate<IRemoveExpiredPrivateRoomsJob>("remove-expired-private-rooms",
                    x => x.Execute(),
                    Cron.Daily, _timeZoneInfo);
                RecurringJob.AddOrUpdate<IRemoveExpiredPremiumsJob>("remove-expired-premiums",
                    x => x.Execute(),
                    Cron.Daily, _timeZoneInfo);
                RecurringJob.AddOrUpdate<INotifyExpiredLoveRoomsJob>("notify-expired-love-rooms",
                    x => x.Execute(),
                    Cron.Daily, _timeZoneInfo);
                RecurringJob.AddOrUpdate<INotifyExpiredPrivateRoomsJob>("notify-expired-private-rooms",
                    x => x.Execute(),
                    Cron.Daily, _timeZoneInfo);
                RecurringJob.AddOrUpdate<INotifyExpiredPremiumsJob>("notify-expired-premiums",
                    x => x.Execute(),
                    Cron.Daily, _timeZoneInfo);

                RecurringJob.AddOrUpdate<IActivityRewardJob>("activity-reward",
                    x => x.Execute(),
                    Cron.Weekly, _timeZoneInfo);

                if (_env.IsDevelopment())
                {
                    _logger.LogInformation(
                        "Env is development, registering commands to guild {GuildId}",
                        _options.GuildId);

                    await request.InteractionService.RegisterCommandsToGuildAsync(_options.GuildId);
                }
                else
                {
                    _logger.LogInformation(
                        "Env is production, registering commands globally");

                    await request.InteractionService.RegisterCommandsGloballyAsync();
                }

                _logger.LogInformation(
                    "Bot started");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,
                    "Unable to startup the bot. Application will now exit");

                _lifetime.StopApplication();
            }

            return new Unit();
        }
    }
}