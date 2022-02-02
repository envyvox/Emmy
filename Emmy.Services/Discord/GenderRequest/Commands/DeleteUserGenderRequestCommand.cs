using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.GenderRequest.Commands
{
    public record DeleteUserGenderRequestCommand(long UserId) : IRequest;

    public class DeleteUserGenderRequestHandler : IRequestHandler<DeleteUserGenderRequestCommand>
    {
        private readonly ILogger<DeleteUserGenderRequestHandler> _logger;
        private readonly AppDbContext _db;

        public DeleteUserGenderRequestHandler(
            DbContextOptions options,
            ILogger<DeleteUserGenderRequestHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteUserGenderRequestCommand request, CancellationToken ct)
        {
            var entity = await _db.UserGenderRequests
                .SingleOrDefaultAsync(x => x.UserId == request.UserId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have gender request entity");
            }

            await _db.DeleteEntity(entity);

            _logger.LogInformation(
                "Deleted user gender request entity {@Entity}",
                entity);

            return Unit.Value;
        }
    }
}