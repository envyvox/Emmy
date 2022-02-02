using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Banner.Commands
{
    public record ActivateUserBannerCommand(long UserId, Guid BannerId) : IRequest;

    public class ActivateUserBannerHandler : IRequestHandler<ActivateUserBannerCommand>
    {
        private readonly ILogger<ActivateUserBannerHandler> _logger;
        private readonly AppDbContext _db;

        public ActivateUserBannerHandler(
            DbContextOptions options,
            ILogger<ActivateUserBannerHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(ActivateUserBannerCommand request, CancellationToken ct)
        {
            var entity = await _db.UserBanners
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.BannerId == request.BannerId);

            entity.IsActive = true;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Activated user {UserId} banner {BannerId}",
                request.UserId, request.BannerId);

            return Unit.Value;
        }
    }
}