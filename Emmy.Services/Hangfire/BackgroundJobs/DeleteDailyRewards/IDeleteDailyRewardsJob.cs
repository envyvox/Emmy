using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.DeleteDailyRewards
{
    public interface IDeleteDailyRewardsJob
    {
        Task Execute();
    }
}