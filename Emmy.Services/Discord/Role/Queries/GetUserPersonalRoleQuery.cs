using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Discord.Role.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.Role.Queries
{
    public record GetUserPersonalRoleQuery(long UserId) : IRequest<UserRoleDto>;

    public class GetUserPersonalRoleHandler : IRequestHandler<GetUserPersonalRoleQuery, UserRoleDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserPersonalRoleHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<UserRoleDto> Handle(GetUserPersonalRoleQuery request, CancellationToken ct)
        {
            var entity = await _db.UserRoles
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.IsPersonal);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have a personal role");
            }

            return _mapper.Map<UserRoleDto>(entity);
        }
    }
}