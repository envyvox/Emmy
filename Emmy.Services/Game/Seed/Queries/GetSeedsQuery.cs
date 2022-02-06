using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Seed.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Seed.Queries
{
    public record GetSeedsQuery : IRequest<List<SeedDto>>;

    public class GetSeedsHandler : IRequestHandler<GetSeedsQuery, List<SeedDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetSeedsHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<SeedDto>> Handle(GetSeedsQuery request, CancellationToken ct)
        {
            var entities = await _db.Seeds
                .Include(x => x.Crop)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return _mapper.Map<List<SeedDto>>(entities);
        }
    }
}