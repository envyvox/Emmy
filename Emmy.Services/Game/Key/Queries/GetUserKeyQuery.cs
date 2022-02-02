using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Game.Key.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Key.Queries
{
    public record GetUserKeyQuery(long UserId, KeyType Type) : IRequest<UserKeyDto>;

    public class GetUserKeyHandler : IRequestHandler<GetUserKeyQuery, UserKeyDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserKeyHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<UserKeyDto> Handle(GetUserKeyQuery request, CancellationToken ct)
        {
            var entity = await _db.UserKeys
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            return entity is null
                ? new UserKeyDto(request.UserId, request.Type, 0)
                : _mapper.Map<UserKeyDto>(entity);
        }
    }
}