using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Fraction.Queries
{
    public record GetFractionUsersCountQuery(Data.Enums.Fraction Fraction) : IRequest<uint>;

    public class GetFractionUsersCountHandler : IRequestHandler<GetFractionUsersCountQuery, uint>
    {
        private readonly AppDbContext _db;

        public GetFractionUsersCountHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<uint> Handle(GetFractionUsersCountQuery request, CancellationToken ct)
        {
            return (uint) await _db.Users
                .CountAsync(x => x.Fraction == request.Fraction);
        }
    }
}