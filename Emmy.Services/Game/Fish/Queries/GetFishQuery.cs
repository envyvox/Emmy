using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Fish.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Fish.Queries
{
    public record GetFishQuery(Guid Id) : IRequest<FishDto>;

    public class GetFishHandler : IRequestHandler<GetFishQuery, FishDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetFishHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<FishDto> Handle(GetFishQuery request, CancellationToken ct)
        {
            var entity = await _db.Fishes
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (entity is null)
            {
                throw new Exception(
                    $"fish {request.Id} not found");
            }

            return _mapper.Map<FishDto>(entity);
        }
    }
}