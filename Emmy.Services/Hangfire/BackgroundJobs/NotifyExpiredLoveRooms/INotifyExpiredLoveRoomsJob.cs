using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredLoveRooms
{
    public interface INotifyExpiredLoveRoomsJob
    {
        Task Execute();
    }
}