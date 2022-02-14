using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Fraction.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Fraction.Queries
{
    public record GetFractionQuery(Data.Enums.Fraction Fraction) : IRequest<FractionDto>;

    public class GetFractionHandler : IRequestHandler<GetFractionQuery, FractionDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetFractionHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<FractionDto> Handle(GetFractionQuery request, CancellationToken ct)
        {
            var entity = await _db.Fractions
                .SingleOrDefaultAsync(x => x.Type == request.Fraction);

            if (entity is null)
            {
                throw new Exception(
                    $"fraction {request.Fraction.ToString()} not found in database");
            }

            return _mapper.Map<FractionDto>(entity);
        }
    }
}