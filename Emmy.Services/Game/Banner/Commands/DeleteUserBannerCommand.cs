using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Game.Banner.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Banner.Commands
{
    public record DeleteUserBannerCommand(long UserId, Guid BannerId) : IRequest;

    public class DeleteUserBannerHandler : IRequestHandler<DeleteUserBannerCommand>
    {
        private readonly ILogger<DeleteUserBannerHandler> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public DeleteUserBannerHandler(
            DbContextOptions options,
            ILogger<DeleteUserBannerHandler> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DeleteUserBannerCommand request, CancellationToken ct)
        {
            var entity = await EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(_db.UserBanners, x =>
                x.UserId == request.UserId &&
                x.BannerId == request.BannerId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have banner {request.BannerId}");
            }

            await _db.DeleteEntity(entity);

            _logger.LogInformation(
                "Deleted user banner entity {@Entity}",
                entity);

            if (entity.IsActive)
            {
                var banners = await _mediator.Send(new GetBannersQuery());
                var banner = banners.Single(x => x.Name == "Ночной город");

                await _mediator.Send(new ActivateUserBannerCommand(request.UserId, banner.Id));
            }

            return Unit.Value;
        }
    }
}