using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Game.Key.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Key.Queries
{
    public record GetKeyQuery(KeyType Type) : IRequest<KeyDto>;

    public class GetKeyHandler : IRequestHandler<GetKeyQuery, KeyDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetKeyHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<KeyDto> Handle(GetKeyQuery request, CancellationToken cancellationToken)
        {
            var entity = await _db.Keys
                .SingleOrDefaultAsync(x => x.Type == request.Type);

            if (entity is null)
            {
                throw new Exception(
                    $"key {request.Type.ToString()} not found in db");
            }

            return _mapper.Map<KeyDto>(entity);
        }
    }
}