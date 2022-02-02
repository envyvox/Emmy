using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.VoiceStatistic
{
    public interface IVoiceStatisticJob
    {
        Task Execute();
    }
}
