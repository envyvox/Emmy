using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Discord.LoveRoom.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.LoveRoom.Queries
{
    public record GetLoveRoomQuery(Guid RelationshipId) : IRequest<LoveRoomDto>;

    public class GetLoveRoomHandler : IRequestHandler<GetLoveRoomQuery, LoveRoomDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetLoveRoomHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<LoveRoomDto> Handle(GetLoveRoomQuery request, CancellationToken ct)
        {
            var entity = await _db.LoveRooms
                .Include(x => x.Relationship)
                .SingleOrDefaultAsync(x => x.RelationshipId == request.RelationshipId);

            if (entity is null)
            {
                throw new Exception(
                    $"relationship {request.RelationshipId} doesnt have love room");
            }

            return _mapper.Map<LoveRoomDto>(entity);
        }
    }
}