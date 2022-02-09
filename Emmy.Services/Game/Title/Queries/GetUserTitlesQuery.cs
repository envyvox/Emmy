using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Title.Queries
{
    public record GetUserTitlesQuery(long UserId) : IRequest<List<Emmy.Data.Enums.Title>>;

    public class GetUserTitlesHandler : IRequestHandler<GetUserTitlesQuery, List<Emmy.Data.Enums.Title>>
    {
        private readonly AppDbContext _db;

        public GetUserTitlesHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<List<Data.Enums.Title>> Handle(GetUserTitlesQuery request, CancellationToken ct)
        {
            var entities = await _db.UserTitles
                .AsQueryable()
                .Where(x => x.UserId == request.UserId)
                .Select(x => x.Type)
                .ToListAsync();

            return entities;
        }
    }
}