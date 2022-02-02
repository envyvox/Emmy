using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using Emmy.Services.Game.World.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.World.Commands
{
    public record UpdateWorldPropertyCommand(WorldProperty Type, uint Value) : IRequest<WorldPropertyDto>;

    public class UpdateWorldPropertyHandler : IRequestHandler<UpdateWorldPropertyCommand, WorldPropertyDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public UpdateWorldPropertyHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _mapper = mapper;
            _db = new AppDbContext(options);
        }

        public async Task<WorldPropertyDto> Handle(UpdateWorldPropertyCommand request, CancellationToken ct)
        {
            var entity = await _db.WorldProperties
                .SingleOrDefaultAsync(x => x.Type == request.Type);

            if (entity is null)
            {
                var created = await _db.CreateEntity(new Emmy.Data.Entities.WorldProperty
                {
                    Type = request.Type,
                    Value = request.Value
                });

                return _mapper.Map<WorldPropertyDto>(created);
            }

            entity.Value = request.Value;

            await _db.UpdateEntity(entity);

            return _mapper.Map<WorldPropertyDto>(entity);
        }
    }
}