using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredLoveRooms
{
    public interface IRemoveExpiredLoveRoomsJob
    {
        Task Execute();
    }
}