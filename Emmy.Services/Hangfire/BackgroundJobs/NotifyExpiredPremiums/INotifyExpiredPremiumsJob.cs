using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredPremiums
{
    public interface INotifyExpiredPremiumsJob
    {
        Task Execute();
    }
}