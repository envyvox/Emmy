using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.ActivityReward
{
    public interface IActivityRewardJob
    {
        Task Execute();
    }
}