using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.CompleteFarmWatering
{
    public interface ICompleteFarmWateringJob
    {
        Task Execute(long userId, uint farmCount);
    }
}