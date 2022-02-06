using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.CommunityDesc.Commands
{
    public record DeleteContentMessageCommand(long ChannelId, long MessageId) : IRequest;

    public class DeleteContentMessageHandler : IRequestHandler<DeleteContentMessageCommand>
    {
        private readonly ILogger<DeleteContentMessageHandler> _logger;
        private readonly AppDbContext _db;

        public DeleteContentMessageHandler(
            DbContextOptions options,
            ILogger<DeleteContentMessageHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(DeleteContentMessageCommand request, CancellationToken ct)
        {
            var entity = await _db.ContentMessages
                .SingleOrDefaultAsync(x =>
                    x.ChannelId == request.ChannelId &&
                    x.MessageId == request.MessageId);

            if (entity is null)
            {
                throw new Exception(
                    $"content message {request.MessageId} in channel {request.ChannelId} not found");
            }

            await _db.DeleteEntity(entity);

            _logger.LogInformation(
                "Deleted content message entity with message {MessageId} in channel {ChannelId}",
                request.MessageId, request.ChannelId);

            return Unit.Value;
        }
    }
}