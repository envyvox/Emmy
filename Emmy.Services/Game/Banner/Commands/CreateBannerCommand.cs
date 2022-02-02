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
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Banner.Commands
{
    public record CreateBannerCommand(
            string Name,
            BannerRarity Rarity,
            uint Price,
            string Url)
        : IRequest<BannerDto>;

    public class CreateBannerHandler : IRequestHandler<CreateBannerCommand, BannerDto>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBannerHandler> _logger;
        private readonly AppDbContext _db;

        public CreateBannerHandler(
            DbContextOptions options,
            IMapper mapper,
            ILogger<CreateBannerHandler> logger)
        {
            _mapper = mapper;
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<BannerDto> Handle(CreateBannerCommand request, CancellationToken cancellationToken)
        {
            var exist = await _db.Banners
                .AnyAsync(x => x.Name == request.Name);

            if (exist)
            {
                throw new Exception(
                    $"banner with name {request.Name} already exist");
            }

            var created = await _db.CreateEntity(new Data.Entities.Banner
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Rarity = request.Rarity,
                Price = request.Price,
                Url = request.Url
            });

            _logger.LogInformation(
                "Created banner {@Banner}",
                created);

            return _mapper.Map<BannerDto>(created);
        }
    }
}