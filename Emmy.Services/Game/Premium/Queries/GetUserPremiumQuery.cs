using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Premium.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Premium.Queries
{
    public record GetUserPremiumQuery(long UserId) : IRequest<UserPremiumDto>;

    public class GetUserPremiumHandler : IRequestHandler<GetUserPremiumQuery, UserPremiumDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserPremiumHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _mapper = mapper;
            _db = new AppDbContext(options);
        }

        public async Task<UserPremiumDto> Handle(GetUserPremiumQuery request, CancellationToken ct)
        {
            var entity = await _db.UserPremiums
                .SingleOrDefaultAsync(x => x.UserId == request.UserId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have a premium entity");
            }

            return _mapper.Map<UserPremiumDto>(entity);
        }
    }
}