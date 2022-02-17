using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Farm.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Farm.Queries
{
    public record GetUserFarmsQuery(long UserId) : IRequest<List<UserFarmDto>>;

    public class GetUserFarmsHandler : IRequestHandler<GetUserFarmsQuery, List<UserFarmDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserFarmsHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<UserFarmDto>> Handle(GetUserFarmsQuery request, CancellationToken ct)
        {
            var entities = await _db.UserFarms
                .Include(x => x.Seed)
                .ThenInclude(x => x.Crop)
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.Number)
                .ToListAsync();

            return _mapper.Map<List<UserFarmDto>>(entities);
        }
    }
}