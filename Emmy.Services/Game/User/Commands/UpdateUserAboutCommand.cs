using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.User.Commands
{
    public record UpdateUserAboutCommand(long UserId, string About) : IRequest;

    public class UpdateUserAboutHandler : IRequestHandler<UpdateUserAboutCommand>
    {
        private readonly ILogger<UpdateUserAboutHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateUserAboutHandler(
            DbContextOptions options,
            ILogger<UpdateUserAboutHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserAboutCommand request, CancellationToken ct)
        {
            var entity = await _db.Users
                .SingleOrDefaultAsync(x => x.Id == request.UserId);

            entity.About = request.About;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated user {UserId} about to {About}",
                request.UserId, request.About);

            return Unit.Value;
        }
    }
}