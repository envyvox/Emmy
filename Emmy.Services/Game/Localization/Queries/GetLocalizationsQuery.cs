using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Localization.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Localization.Queries
{
    public record GetLocalizationsQuery : IRequest<List<LocalizationDto>>;

    public class GetLocalizationsHandler : IRequestHandler<GetLocalizationsQuery, List<LocalizationDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetLocalizationsHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<LocalizationDto>> Handle(GetLocalizationsQuery request, CancellationToken ct)
        {
            var entities = await _db.Localizations
                .AsQueryable()
                .OrderBy(x => x.Category)
                .ToListAsync();

            return _mapper.Map<List<LocalizationDto>>(entities);
        }
    }
}