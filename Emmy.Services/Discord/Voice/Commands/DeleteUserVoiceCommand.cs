using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Voice.Commands
{
    public record DeleteUserVoiceCommand(long UserId) : IRequest;

    public class DeleteUserVoiceHandler : IRequestHandler<DeleteUserVoiceCommand>
    {
        private readonly ILogger<DeleteUserVoiceHandler> _logger;
        private readonly AppDbContext _db;

        public DeleteUserVoiceHandler(
            DbContextOptions options,
            ILogger<DeleteUserVoiceHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteUserVoiceCommand request, CancellationToken ct)
        {
            var entity = await _db.UserVoices
                .SingleOrDefaultAsync(x => x.UserId == request.UserId);

            if (entity is null)
            {
                throw new Exception(
                    $"User {request.UserId} doesnt have voice entity");
            }

            await _db.DeleteEntity(entity);

            _logger.LogInformation(
                "Deleted user voice entity for user {UserId}",
                request.UserId);

            return Unit.Value;
        }
    }
}