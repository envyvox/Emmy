using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Seed.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Seed.Queries
{
    public record GetSeedQuery(Guid Id) : IRequest<SeedDto>;

    public class GetSeedHandler : IRequestHandler<GetSeedQuery, SeedDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetSeedHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<SeedDto> Handle(GetSeedQuery request, CancellationToken ct)
        {
            var entity = await _db.Seeds
                .Include(x => x.Crop)
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (entity is null)
            {
                throw new Exception(
                    $"seed {request.Id} not found");
            }

            return _mapper.Map<SeedDto>(entity);
        }
    }
}