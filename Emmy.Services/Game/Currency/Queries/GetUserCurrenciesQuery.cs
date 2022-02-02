using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Currency.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Currency.Queries
{
    public record GetUserCurrenciesQuery(long UserId) : IRequest<Dictionary<Data.Enums.Currency, UserCurrencyDto>>;

    public class GetUserCurrenciesHandler
        : IRequestHandler<GetUserCurrenciesQuery, Dictionary<Data.Enums.Currency, UserCurrencyDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserCurrenciesHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<Dictionary<Data.Enums.Currency, UserCurrencyDto>> Handle(
            GetUserCurrenciesQuery request, CancellationToken ct)
        {
            var entities = await _db.UserCurrencies
                .AsQueryable()
                .Where(x => x.UserId == request.UserId)
                .ToDictionaryAsync(x => x.Type);

            return _mapper.Map<Dictionary<Data.Enums.Currency, UserCurrencyDto>>(entities);
        }
    }
}