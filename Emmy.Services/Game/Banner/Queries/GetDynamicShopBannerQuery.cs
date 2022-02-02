using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Banner.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Game.Banner.Queries
{
    public record GetDynamicShopBannerQuery(Guid Id) : IRequest<BannerDto>;

    public class GetDynamicShopBannerByIncIdHandler : IRequestHandler<GetDynamicShopBannerQuery, BannerDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetDynamicShopBannerByIncIdHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<BannerDto> Handle(GetDynamicShopBannerQuery request, CancellationToken ct)
        {
            var entity = await _db.DynamicShopBanners
                .Include(x => x.Banner)
                .Select(x => x.Banner)
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (entity is null)
            {
                throw new GameUserExpectedException(
                    "в магазине нет баннера с таким номером.");
            }

            return _mapper.Map<BannerDto>(entity);
        }
    }
}