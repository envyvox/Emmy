using System.Collections.Generic;
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
    public record GetDynamicShopBannersQuery : IRequest<List<BannerDto>>;

    public class GetDynamicShopBannersHandler : IRequestHandler<GetDynamicShopBannersQuery, List<BannerDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetDynamicShopBannersHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<BannerDto>> Handle(GetDynamicShopBannersQuery request, CancellationToken ct)
        {
            var entities = await _db.DynamicShopBanners
                .Include(x => x.Banner)
                .Select(x => x.Banner)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return _mapper.Map<List<BannerDto>>(entities);
        }
    }
}