using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.CompleteFishing
{
    public interface ICompleteFishingJob
    {
        Task Execute(long userId);
    }
}