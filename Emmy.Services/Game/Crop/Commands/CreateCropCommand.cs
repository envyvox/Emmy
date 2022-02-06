using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Game.Crop.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Crop.Commands
{
    public record CreateCropCommand(string Name, uint Price, Guid SeedId) : IRequest<CropDto>;

    public class CreateCropHandler : IRequestHandler<CreateCropCommand, CropDto>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCropHandler> _logger;
        private readonly AppDbContext _db;

        public CreateCropHandler(
            DbContextOptions options,
            IMapper mapper,
            ILogger<CreateCropHandler> logger)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CropDto> Handle(CreateCropCommand request, CancellationToken ct)
        {
            var exist = await _db.Crops
                .AnyAsync(x => x.Name == request.Name);

            if (exist)
            {
                throw new Exception(
                    $"crop with name {request.Name} already exist");
            }

            var created = await _db.CreateEntity(new Data.Entities.Crop
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                SeedId = request.SeedId
            });

            _logger.LogInformation(
                "Created crop entity {@Entity}",
                created);

            return _mapper.Map<CropDto>(created);
        }
    }
}