using System.Threading.Tasks;
using Emmy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.BackgroundJobs.DeleteDailyRewards
{
    public class DeleteDailyRewardsJob : IDeleteDailyRewardsJob
    {
        private readonly ILogger<DeleteDailyRewardsJob> _logger;
        private readonly AppDbContext _db;

        public DeleteDailyRewardsJob(
            ILogger<DeleteDailyRewardsJob> logger,
            DbContextOptions options)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Delete daily rewards job executed");

            await _db.Database.ExecuteSqlRawAsync("truncate user_daily_rewards;");
        }
    }
}