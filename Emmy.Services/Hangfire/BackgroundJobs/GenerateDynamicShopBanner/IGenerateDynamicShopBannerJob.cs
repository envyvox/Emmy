using System.Threading.Tasks;

namespace Emmy.Services.Hangfire.BackgroundJobs.GenerateDynamicShopBanner
{
    public interface IGenerateDynamicShopBannerJob
    {
        Task Execute();
    }
}
