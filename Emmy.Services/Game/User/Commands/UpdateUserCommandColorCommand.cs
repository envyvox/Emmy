using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.User.Commands
{
    public record UpdateUserCommandColorCommand(long UserId, string CommandColor) : IRequest;

    public class UpdateUserCommandColorHandler : IRequestHandler<UpdateUserCommandColorCommand>
    {
        private readonly ILogger<UpdateUserCommandColorHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateUserCommandColorHandler(
            DbContextOptions options,
            ILogger<UpdateUserCommandColorHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(UpdateUserCommandColorCommand request, CancellationToken ct)
        {
            var entity = await _db.Users
                .SingleOrDefaultAsync(x => x.Id == request.UserId);

            entity.CommandColor = request.CommandColor;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated user {UserId} command color to {CommandColor}",
                request.UserId, request.CommandColor);

            return Unit.Value;
        }
    }
}