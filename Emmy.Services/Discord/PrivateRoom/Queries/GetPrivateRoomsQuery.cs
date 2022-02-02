using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Discord.PrivateRoom.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.PrivateRoom.Queries
{
    public record GetPrivateRoomsQuery : IRequest<List<PrivateRoomDto>>;

    public class GetPrivateRoomsHandler : IRequestHandler<GetPrivateRoomsQuery, List<PrivateRoomDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetPrivateRoomsHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<PrivateRoomDto>> Handle(GetPrivateRoomsQuery request, CancellationToken ct)
        {
            var entities = await _db.PrivateRooms
                .Include(x => x.User)
                .ToListAsync();

            return _mapper.Map<List<PrivateRoomDto>>(entities);
        }
    }
}