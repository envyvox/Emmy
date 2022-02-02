using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Banner.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Banner.Queries
{
    public record GetUserActiveBannerQuery(long UserId) : IRequest<BannerDto>;

    public class GetUserActiveBannerHandler : IRequestHandler<GetUserActiveBannerQuery, BannerDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserActiveBannerHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<BannerDto> Handle(GetUserActiveBannerQuery request, CancellationToken ct)
        {
            var entity = await _db.UserBanners
                .Include(x => x.Banner)
                .Where(x => x.UserId == request.UserId && x.IsActive)
                .Select(x => x.Banner)
                .SingleOrDefaultAsync();

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have active banner.");
            }

            return _mapper.Map<BannerDto>(entity);
        }
    }
}