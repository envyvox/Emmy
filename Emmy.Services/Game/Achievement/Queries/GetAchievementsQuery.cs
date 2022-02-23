using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Game.Achievement.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Achievement.Queries
{
    public record GetAchievementsQuery(AchievementCategory Category) : IRequest<List<AchievementDto>>;

    public class GetAchievementsHandler : IRequestHandler<GetAchievementsQuery, List<AchievementDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetAchievementsHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<AchievementDto>> Handle(GetAchievementsQuery request, CancellationToken ct)
        {
            var entities = await _db.Achievements
                .AsQueryable()
                .OrderByDescending(x => x.CreatedAt)
                .Where(x => x.Category == request.Category)
                .ToListAsync();

            return _mapper.Map<List<AchievementDto>>(entities);
        }
    }
}