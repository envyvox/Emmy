using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.ShopRole.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.ShopRole.Queries
{
    public record GetShopRoleQuery(long RoleId) : IRequest<ShopRoleDto>;

    public class GetShopRoleHandler : IRequestHandler<GetShopRoleQuery, ShopRoleDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetShopRoleHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<ShopRoleDto> Handle(GetShopRoleQuery request, CancellationToken ct)
        {
            var entity = await _db.ShopRoles
                .SingleOrDefaultAsync(x => x.RoleId == request.RoleId);

            if (entity is null)
            {
                throw new Exception(
                    $"role {request.RoleId} not found in shop");
            }

            return _mapper.Map<ShopRoleDto>(entity);
        }
    }
}