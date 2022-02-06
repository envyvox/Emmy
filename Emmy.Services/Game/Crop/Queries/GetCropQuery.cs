using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Crop.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Crop.Queries
{
    public record GetCropQuery(Guid Id) : IRequest<CropDto>;

    public class GetCropHandler : IRequestHandler<GetCropQuery, CropDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetCropHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<CropDto> Handle(GetCropQuery request, CancellationToken ct)
        {
            var entity = await _db.Crops
                .Include(x => x.Seed)
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (entity is null)
            {
                throw new Exception(
                    $"crop {request.Id} not found");
            }

            return _mapper.Map<CropDto>(entity);
        }
    }
}