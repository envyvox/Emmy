using System.Threading.Tasks;
using Emmy.Data.Enums;

namespace Emmy.Services.Hangfire.BackgroundJobs.CompleteTransit
{
    public interface ICompleteTransitJob
    {
        Task Execute(long userId, Location destination);
    }
}