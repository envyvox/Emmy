using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.CommunityDesc.Queries
{
    public record GetContentAuthorVotesCountQuery(long UserId, Vote Vote) : IRequest<uint>;

    public class GetContentAuthorVotesCountHandler : IRequestHandler<GetContentAuthorVotesCountQuery, uint>
    {
        private readonly AppDbContext _db;

        public GetContentAuthorVotesCountHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<uint> Handle(GetContentAuthorVotesCountQuery request, CancellationToken ct)
        {
            return (uint) await _db.ContentVotes
                .Include(x => x.ContentMessage)
                .CountAsync(x =>
                    x.ContentMessage.UserId == request.UserId &&
                    x.Vote == request.Vote &&
                    x.IsActive);
        }
    }
}