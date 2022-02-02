using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Key.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Key.Queries
{
    public record GetKeysQuery : IRequest<List<KeyDto>>;

    public class GetKeysHandler : IRequestHandler<GetKeysQuery, List<KeyDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetKeysHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<KeyDto>> Handle(GetKeysQuery request, CancellationToken ct)
        {
            var entities = await _db.Keys
                .AsQueryable()
                .OrderBy(x => x.Type)
                .ToListAsync();

            return _mapper.Map<List<KeyDto>>(entities);
        }
    }
}