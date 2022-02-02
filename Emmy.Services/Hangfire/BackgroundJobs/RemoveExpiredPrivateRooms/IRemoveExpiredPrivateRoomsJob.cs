using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredPrivateRooms
{
    public interface IRemoveExpiredPrivateRoomsJob
    {
        Task Execute();
    }
}