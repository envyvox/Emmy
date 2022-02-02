using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.World.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.World.Queries
{
    public record GetWorldPropertiesQuery : IRequest<List<WorldPropertyDto>>;

    public class GetWorldPropertiesHandler : IRequestHandler<GetWorldPropertiesQuery, List<WorldPropertyDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetWorldPropertiesHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<WorldPropertyDto>> Handle(GetWorldPropertiesQuery request, CancellationToken ct)
        {
            var entities = await _db.WorldProperties
                .AsQueryable()
                .OrderBy(x => x.Type)
                .ToListAsync();

            return _mapper.Map<List<WorldPropertyDto>>(entities);
        }
    }
}