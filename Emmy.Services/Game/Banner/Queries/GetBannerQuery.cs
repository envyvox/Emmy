using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Banner.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Banner.Queries
{
    public record GetBannerQuery(Guid Id) : IRequest<BannerDto>;

    public class GetBannerHandler : IRequestHandler<GetBannerQuery, BannerDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetBannerHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<BannerDto> Handle(GetBannerQuery request, CancellationToken ct)
        {
            var entity = await _db.Banners
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (entity is null)
            {
                throw new Exception(
                    $"banner with id {request.Id} not found");
            }

            return _mapper.Map<BannerDto>(entity);
        }
    }
}