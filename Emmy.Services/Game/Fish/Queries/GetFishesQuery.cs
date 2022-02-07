using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Fish.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Fish.Queries
{
    public record GetFishesQuery : IRequest<List<FishDto>>;

    public class GetFishesHandler : IRequestHandler<GetFishesQuery, List<FishDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetFishesHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<FishDto>> Handle(GetFishesQuery request, CancellationToken ct)
        {
            var entities = await _db.Fishes
                .AsQueryable()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return _mapper.Map<List<FishDto>>(entities);
        }
    }
}