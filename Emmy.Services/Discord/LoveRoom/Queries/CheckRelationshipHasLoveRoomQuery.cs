using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.LoveRoom.Queries
{
    public record CheckRelationshipHasLoveRoomQuery(Guid RelationshipId) : IRequest<bool>;

    public class CheckRelationshipHasLoveRoomHandler : IRequestHandler<CheckRelationshipHasLoveRoomQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckRelationshipHasLoveRoomHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckRelationshipHasLoveRoomQuery request, CancellationToken ct)
        {
            return await _db.LoveRooms
                .AnyAsync(x => x.RelationshipId == request.RelationshipId);
        }
    }
}