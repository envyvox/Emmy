using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using Emmy.Services.Game.Banner.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Banner.Commands
{
    public record UpdateBannerCommand(
            Guid Id,
            string Name,
            BannerRarity Rarity,
            uint Price,
            string Url)
        : IRequest<BannerDto>;

    public class UpdateBannerHandler : IRequestHandler<UpdateBannerCommand, BannerDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public UpdateBannerHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<BannerDto> Handle(UpdateBannerCommand request, CancellationToken cancellationToken)
        {
            var entity = await _db.Banners
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            entity.Name = request.Name;
            entity.Rarity = request.Rarity;
            entity.Price = request.Price;
            entity.Url = request.Url;

            await _db.UpdateEntity(entity);

            return _mapper.Map<BannerDto>(entity);
        }
    }
}