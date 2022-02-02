using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Game.Localization.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Localization.Queries
{
    public record GetLocalizationsByCategoryQuery(
            LocalizationCategory Category)
        : IRequest<List<LocalizationDto>>;

    public class GetLocalizationsByCategoryHandler
        : IRequestHandler<GetLocalizationsByCategoryQuery, List<LocalizationDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetLocalizationsByCategoryHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<LocalizationDto>> Handle(GetLocalizationsByCategoryQuery request, CancellationToken ct)
        {
            var entities = await _db.Localizations
                .AsQueryable()
                .Where(x => x.Category == request.Category)
                .ToListAsync();

            return _mapper.Map<List<LocalizationDto>>(entities);
        }
    }
}