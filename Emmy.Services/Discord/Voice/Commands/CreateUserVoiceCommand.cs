using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.Discord;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Voice.Commands
{
    public record CreateUserVoiceCommand(long UserId, long ChannelId) : IRequest;

    public class CreateUserVoiceHandler : IRequestHandler<CreateUserVoiceCommand>
    {
        private readonly ILogger<CreateUserVoiceHandler> _logger;
        private readonly AppDbContext _db;

        public CreateUserVoiceHandler(
            DbContextOptions options,
            ILogger<CreateUserVoiceHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateUserVoiceCommand request, CancellationToken ct)
        {
            var exist = await _db.UserVoices
                .AnyAsync(x => x.UserId == request.UserId);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already have voice entity");
            }

            await _db.CreateEntity(new UserVoice
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ChannelId = request.ChannelId
            });

            _logger.LogInformation(
                "Created user voice entity for user {UserId} in channel {ChannelId}",
                request.UserId, request.ChannelId);

            return Unit.Value;
        }
    }
}