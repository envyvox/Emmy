using System;
using System.Linq;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Services.Game.Premium.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredPremiums
{
    public class RemoveExpiredPremiumsJob : IRemoveExpiredPremiumsJob
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RemoveExpiredPremiumsJob> _logger;
        private readonly AppDbContext _db;

        public RemoveExpiredPremiumsJob(
            DbContextOptions options,
            IMediator mediator,
            ILogger<RemoveExpiredPremiumsJob> logger)
        {
            _db = new AppDbContext(options);
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Remove expired premiums job executed");

            var timeNow = DateTimeOffset.UtcNow;
            var entities = await _db.UserPremiums
                .AsQueryable()
                .Where(x => x.Expiration < timeNow)
                .ToListAsync();

            foreach (var entity in entities)
            {
                await _mediator.Send(new RemovePremiumFromUserCommand(entity.UserId));
            }
        }
    }
}