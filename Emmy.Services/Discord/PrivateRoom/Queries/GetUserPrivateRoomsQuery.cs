using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Discord.PrivateRoom.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.PrivateRoom.Queries
{
    public record GetUserPrivateRoomsQuery(long UserId) : IRequest<List<PrivateRoomDto>>;

    public class GetUserPrivateRoomsHandler : IRequestHandler<GetUserPrivateRoomsQuery, List<PrivateRoomDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserPrivateRoomsHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<PrivateRoomDto>> Handle(GetUserPrivateRoomsQuery request, CancellationToken ct)
        {
            var entities = await _db.PrivateRooms
                .Include(x => x.User)
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<PrivateRoomDto>>(entities);
        }
    }
}