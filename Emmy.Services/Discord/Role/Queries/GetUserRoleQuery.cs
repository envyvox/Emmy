using System;
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
    public record GetUserRoleQuery(long UserId, long RoleId) : IRequest<UserRoleDto>;

    public class GetUserRoleHandler : IRequestHandler<GetUserRoleQuery, UserRoleDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserRoleHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<UserRoleDto> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
        {
            var entity = await _db.UserRoles
                .AsQueryable()
                .Where(x =>
                    x.UserId == request.UserId &&
                    x.RoleId == request.RoleId)
                .SingleOrDefaultAsync();

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have role {request.RoleId}");
            }

            return _mapper.Map<UserRoleDto>(entity);
        }
    }
}