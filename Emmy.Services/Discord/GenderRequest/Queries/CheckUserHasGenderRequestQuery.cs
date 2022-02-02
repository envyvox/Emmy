using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.GenderRequest.Queries
{
    public record CheckUserHasGenderRequestQuery(long UserId) : IRequest<bool>;

    public class CheckUserHasGenderRequestHandler : IRequestHandler<CheckUserHasGenderRequestQuery, bool>
    {
        private readonly AppDbContext _db;

        public CheckUserHasGenderRequestHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<bool> Handle(CheckUserHasGenderRequestQuery request, CancellationToken ct)
        {
            return await _db.UserGenderRequests
                .AnyAsync(x => x.UserId == request.UserId);
        }
    }
}