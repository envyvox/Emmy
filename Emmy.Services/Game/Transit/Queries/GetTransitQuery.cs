using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Game.Transit.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Transit.Queries
{
    public record GetTransitQuery(Location Departure, Location Destination) : IRequest<TransitDto>;

    public class GetTransitHandler : IRequestHandler<GetTransitQuery, TransitDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetTransitHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<TransitDto> Handle(GetTransitQuery request, CancellationToken ct)
        {
            var entity = await _db.Transits
                .SingleOrDefaultAsync(x =>
                    x.Departure == request.Departure &&
                    x.Destination == request.Destination);

            if (entity is null)
            {
                throw new Exception(
                    $"transit from {request.Departure.ToString()} to {request.Destination.ToString()} doesnt exist");
            }

            return _mapper.Map<TransitDto>(entity);
        }
    }
}