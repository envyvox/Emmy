using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Hangfire.BackgroundJobs.VoiceStatistic
{
    public class VoiceStatisticJob : IVoiceStatisticJob
    {
        private readonly AppDbContext _db;

        public VoiceStatisticJob(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task Execute()
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(@$"
                insert into user_statistics as us (id, type, amount, created_at, updated_at, user_id)
                select uuid_generate_v4(), {Statistic.MinutesInVoice}, 1, now(), now(), user_id from user_voices
                on conflict (user_id, type) do update set amount = us.amount + 1;");
        }
    }
}