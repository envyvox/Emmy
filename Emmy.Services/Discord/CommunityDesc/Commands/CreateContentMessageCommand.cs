using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.Discord;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.CommunityDesc.Commands
{
    public record CreateContentMessageCommand(long UserId, long ChannelId, long MessageId) : IRequest;

    public class CreateContentMessageHandler : IRequestHandler<CreateContentMessageCommand>
    {
        private readonly AppDbContext _db;

        public CreateContentMessageHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(CreateContentMessageCommand request, CancellationToken ct)
        {
            var exist = await _db.ContentMessages
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.ChannelId == request.ChannelId &&
                    x.MessageId == request.MessageId);

            if (exist)
            {
                throw new Exception(
                    $"content message {request.MessageId} from user {request.UserId} in channel {request.ChannelId} already exist");
            }

            await _db.CreateEntity(new ContentMessage
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ChannelId = request.ChannelId,
                MessageId = request.MessageId,
                CreatedAt = DateTimeOffset.UtcNow
            });

            return Unit.Value;
        }
    }
}
