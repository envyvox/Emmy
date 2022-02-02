using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.GenderRequest.Commands
{
    public record UpdateUserGenderRequestCommand(long UserId, RequestState State, long ModeratorId) : IRequest;

    public class UpdateUserGenderRequestHandler : IRequestHandler<UpdateUserGenderRequestCommand>
    {
        private readonly ILogger<UpdateUserGenderRequestHandler> _logger;
        private readonly AppDbContext _db;

        public UpdateUserGenderRequestHandler(
            DbContextOptions options,
            ILogger<UpdateUserGenderRequestHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserGenderRequestCommand request, CancellationToken ct)
        {
            var entity = await _db.UserGenderRequests
                .SingleOrDefaultAsync(x => x.UserId == request.UserId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have gender request entity");
            }

            entity.State = request.State;
            entity.ModeratorId = request.ModeratorId;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Updated user gender request entity {@Entity}",
                entity);

            return Unit.Value;
        }
    }
}