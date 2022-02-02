using System;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Statistic.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.BackgroundJobs.ActivityReward
{
    public class ActivityRewardJob : IActivityRewardJob
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ActivityRewardJob> _logger;
        private readonly AppDbContext _db;

        private const uint ActivityTokenPerMessage = 5;
        private const uint ActivityTokenPerMinuteInVoice = 2;
        private const double ActivityTokenPerLobbs = 262.0;

        public ActivityRewardJob(
            IMediator mediator,
            ILogger<ActivityRewardJob> logger,
            DbContextOptions options)
        {
            _db = new AppDbContext(options);
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Activity reward job executed");

            var users = await _mediator.Send(new GetUsersQuery());

            foreach (var user in users)
            {
                var statistics = await _mediator.Send(new GetUserStatisticsQuery(user.Id));
                uint activityTokens = 0;

                foreach (var (type, userStatistic) in statistics)
                {
                    activityTokens += userStatistic.Amount * type switch
                    {
                        Statistic.Messages => ActivityTokenPerMessage,
                        Statistic.MinutesInVoice => ActivityTokenPerMinuteInVoice,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }

                var lobbsCount = (uint) Math.Ceiling(activityTokens / ActivityTokenPerLobbs);

                await _mediator.Send(new AddCurrencyToUserCommand(user.Id, Currency.Lobbs, lobbsCount));
            }

            await _db.Database.ExecuteSqlRawAsync("truncate user_statistics;");
            
            _logger.LogInformation(
                "Deleted user statistics from db");
        }
    }
}