using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.StartNewDay
{
    public interface IStartNewDayJob
    {
        Task Execute();
    }
}