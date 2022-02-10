using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Title.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Title.Queries
{
    public record GetUserTitlesQuery(long UserId) : IRequest<List<UserTitleDto>>;

    public class GetUserTitlesHandler : IRequestHandler<GetUserTitlesQuery, List<UserTitleDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserTitlesHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _mapper = mapper;
            _db = new AppDbContext(options);
        }

        public async Task<List<UserTitleDto>> Handle(GetUserTitlesQuery request,
            CancellationToken ct)
        {
            var entities = await _db.UserTitles
                .AsQueryable()
                .OrderByDescending(x => x.CreatedAt)
                .Where(x => x.UserId == request.UserId)
                .ToListAsync();

            return _mapper.Map<List<UserTitleDto>>(entities);
        }
    }
}