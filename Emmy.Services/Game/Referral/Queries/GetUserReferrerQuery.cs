using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.User.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Referral.Queries
{
    public record GetUserReferrerQuery(long UserId) : IRequest<UserDto>;

    public class GetUserReferrerHandler : IRequestHandler<GetUserReferrerQuery, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserReferrerHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserReferrerQuery request, CancellationToken ct)
        {
            var entity = await _db.UserReferrers
                .Include(x => x.Referrer)
                .Where(x => x.UserId == request.UserId)
                .Select(x => x.Referrer)
                .SingleOrDefaultAsync();

            return _mapper.Map<UserDto>(entity);
        }
    }
}