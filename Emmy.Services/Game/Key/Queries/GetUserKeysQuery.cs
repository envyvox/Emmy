using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Game.Key.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Key.Queries
{
    public record GetUserKeysQuery(long UserId) : IRequest<List<UserKeyDto>>;

    public class GetUserKeysHandler : IRequestHandler<GetUserKeysQuery, List<UserKeyDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserKeysHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<UserKeyDto>> Handle(GetUserKeysQuery request, CancellationToken ct)
        {
            var entities = await _db.UserKeys
                .AmountNotZero()
                .Where(x => x.UserId == request.UserId)
                .ToListAsync();

            return _mapper.Map<List<UserKeyDto>>(entities);
        }
    }
}