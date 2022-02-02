using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Currency.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Currency.Queries
{
    public record GetUserCurrencyQuery(long UserId, Data.Enums.Currency Type) : IRequest<UserCurrencyDto>;

    public class GetUserCurrencyHandler : IRequestHandler<GetUserCurrencyQuery, UserCurrencyDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserCurrencyHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _mapper = mapper;
            _db = new AppDbContext(options);
        }

        public async Task<UserCurrencyDto> Handle(GetUserCurrencyQuery request, CancellationToken ct)
        {
            var entity = await _db.UserCurrencies
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            return entity is null
                ? new UserCurrencyDto(request.Type, 0)
                : _mapper.Map<UserCurrencyDto>(entity);
        }
    }
}