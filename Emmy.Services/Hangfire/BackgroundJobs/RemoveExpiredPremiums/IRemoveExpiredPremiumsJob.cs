using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredPremiums
{
    public interface IRemoveExpiredPremiumsJob
    {
        Task Execute();
    }
}