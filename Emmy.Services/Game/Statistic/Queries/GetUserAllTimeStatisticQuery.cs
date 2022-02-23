using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Statistic.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Statistic.Queries
{
    public record GetUserAllTimeStatisticQuery(long UserId, Data.Enums.Statistic Type) : IRequest<UserStatisticDto>;

    public class GetUserAllTimeStatisticHandler : IRequestHandler<GetUserAllTimeStatisticQuery, UserStatisticDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserAllTimeStatisticHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<UserStatisticDto> Handle(GetUserAllTimeStatisticQuery request, CancellationToken ct)
        {
            var entity = await _db.UserAllTimeStatistics
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            return entity is null
                ? new UserStatisticDto(request.Type, 0)
                : _mapper.Map<UserStatisticDto>(entity);
        }
    }
}