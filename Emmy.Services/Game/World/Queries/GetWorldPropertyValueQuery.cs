using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.World.Queries
{
    public record GetWorldPropertyValueQuery(WorldProperty Type) : IRequest<uint>;

    public class GetWorldPropertyValueHandler : IRequestHandler<GetWorldPropertyValueQuery, uint>
    {
        private readonly AppDbContext _db;

        public GetWorldPropertyValueHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<uint> Handle(GetWorldPropertyValueQuery request, CancellationToken ct)
        {
            var entity = await _db.WorldProperties
                .SingleOrDefaultAsync(x => x.Type == request.Type);

            if (entity is null)
            {
                throw new Exception(
                    $"world property {request.Type.ToString()} doesnt exist");
            }

            return entity.Value;
        }
    }
}