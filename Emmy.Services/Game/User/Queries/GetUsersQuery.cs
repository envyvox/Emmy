using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.User.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.User.Queries
{
    public record GetUsersQuery : IRequest<List<UserDto>>;

    public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserDto>>

    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUsersHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
        {
            var entities = await _db.Users
                .ToListAsync();

            return _mapper.Map<List<UserDto>>(entities);
        }
    }
}