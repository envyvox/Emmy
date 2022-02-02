using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredPrivateRooms
{
    public interface INotifyExpiredPrivateRoomsJob
    {
        Task Execute();
    }
}