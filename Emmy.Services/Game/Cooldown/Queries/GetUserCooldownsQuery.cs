using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Cooldown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Cooldown.Queries
{
    public record GetUserCooldownsQuery(long UserId) : IRequest<List<UserCooldownDto>>;

    public class GetUserCooldownsHandler : IRequestHandler<GetUserCooldownsQuery, List<UserCooldownDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserCooldownsHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<UserCooldownDto>> Handle(GetUserCooldownsQuery request, CancellationToken ct)
        {
            var entities = await _db.UserCooldowns
                .AsQueryable()
                .Where(x => x.UserId == request.UserId)
                .ToListAsync();

            return _mapper.Map<List<UserCooldownDto>>(entities);
        }
    }
}