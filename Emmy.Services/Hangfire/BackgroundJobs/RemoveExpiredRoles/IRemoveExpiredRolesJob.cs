using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredRoles
{
    public interface IRemoveExpiredRolesJob
    {
        Task Execute();
    }
}