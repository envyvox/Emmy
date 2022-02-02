using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.User.Commands
{
    public record UpdateUserGenderCommand(long UserId, Gender Gender) : IRequest;

    public class UpdateUserGenderHandler : IRequestHandler<UpdateUserGenderCommand>
    {
        private readonly ILogger<UpdateUserGenderHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateUserGenderHandler(
            DbContextOptions options,
            ILogger<UpdateUserGenderHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserGenderCommand request, CancellationToken ct)
        {
            var entity = await _db.Users
                .SingleOrDefaultAsync(x => x.Id == request.UserId);

            entity.Gender = request.Gender;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated user {UserId} gender to {Gender}",
                request.UserId, request.Gender.ToString());

            return Unit.Value;
        }
    }
}