using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Relationship.Queries
{
    public record CheckUserHasRelationshipQuery(long UserId) : IRequest<bool>;

    public class CheckUserHasRelationshipHandler : IRequestHandler<CheckUserHasRelationshipQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckUserHasRelationshipHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckUserHasRelationshipQuery request, CancellationToken ct)
        {
            return await _db.Relationships
                .AnyAsync(x =>
                    x.User1Id == request.UserId ||
                    x.User2Id == request.UserId);
        }
    }
}