using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Discord.Role.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.Role.Queries
{
    public record GetUserRolesQuery(long UserId) : IRequest<List<UserRoleDto>>;

    public class GetUserRolesHandler : IRequestHandler<GetUserRolesQuery, List<UserRoleDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserRolesHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<UserRoleDto>> Handle(GetUserRolesQuery request, CancellationToken ct)
        {
            var entities = await _db.UserRoles
                .AsQueryable()
                .Where(x => x.UserId == request.UserId)
                .ToListAsync();

            return _mapper.Map<List<UserRoleDto>>(entities);
        }
    }
}