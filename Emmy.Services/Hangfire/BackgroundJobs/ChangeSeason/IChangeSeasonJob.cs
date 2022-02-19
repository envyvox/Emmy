using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.ChangeSeason
{
    public interface IChangeSeasonJob
    {
        Task Execute();
    }
}