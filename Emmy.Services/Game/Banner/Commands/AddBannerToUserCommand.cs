using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Banner.Commands
{
    public record AddBannerToUserCommand(
            long UserId,
            Guid BannerId,
            bool IsActive = false)
        : IRequest;

    public class AddBannerToUserHandler : IRequestHandler<AddBannerToUserCommand>
    {
        private readonly ILogger<AddBannerToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddBannerToUserHandler(
            DbContextOptions options,
            ILogger<AddBannerToUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(AddBannerToUserCommand request, CancellationToken ct)
        {
            var exist = await _db.UserBanners
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.BannerId == request.BannerId);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already have banner {request.BannerId}");
            }

            await _db.CreateEntity(new UserBanner
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                BannerId = request.BannerId,
                IsActive = request.IsActive,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Added user {UserId} banner {BannerId} with active {Active}",
                request.UserId, request.BannerId, request.IsActive);

            return Unit.Value;
        }
    }
}