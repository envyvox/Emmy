using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Cooldown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Cooldown.Queries
{
    public record GetUserCooldownQuery(long UserId, Data.Enums.Cooldown Type) : IRequest<UserCooldownDto>;

    public class GetUserCooldownHandler : IRequestHandler<GetUserCooldownQuery, UserCooldownDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserCooldownHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<UserCooldownDto> Handle(GetUserCooldownQuery request, CancellationToken ct)
        {
            var entity = await _db.UserCooldowns
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            return entity is null
                ? new UserCooldownDto(request.UserId, request.Type, DateTimeOffset.UtcNow)
                : _mapper.Map<UserCooldownDto>(entity);
        }
    }
}