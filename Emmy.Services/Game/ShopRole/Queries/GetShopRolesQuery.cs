using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.ShopRole.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.ShopRole.Queries
{
    public record GetShopRolesQuery : IRequest<List<ShopRoleDto>>;

    public class GetShopRolesHandler : IRequestHandler<GetShopRolesQuery, List<ShopRoleDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetShopRolesHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<ShopRoleDto>> Handle(GetShopRolesQuery request, CancellationToken ct)
        {
            var entities = await _db.ShopRoles
                .ToListAsync();

            return _mapper.Map<List<ShopRoleDto>>(entities);
        }
    }
}