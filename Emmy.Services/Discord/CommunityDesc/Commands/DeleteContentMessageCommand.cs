using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.CommunityDesc.Commands
{
    public record DeleteContentMessageCommand(long ChannelId, long MessageId) : IRequest;

    public class DeleteContentMessageHandler : IRequestHandler<DeleteContentMessageCommand>
    {
        private readonly AppDbContext _db;

        public DeleteContentMessageHandler(DbContextOptions options)
        {
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
                throw new Exception($"content message {request.MessageId} in channel {request.ChannelId} not found");
            }

            await _db.DeleteEntity(entity);

            return Unit.Value;
        }
    }
}