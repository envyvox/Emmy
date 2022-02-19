using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Game.User.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Fraction.Queries
{
    public record GetRandomFractionUserQuery(Data.Enums.Fraction Fraction, long ExceptId) : IRequest<UserDto>;

    public class GetRandomFractionUserHandler : IRequestHandler<GetRandomFractionUserQuery, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetRandomFractionUserHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetRandomFractionUserQuery request, CancellationToken ct)
        {
            var entity = await _db.Users
                .OrderByRandom()
                .FirstOrDefaultAsync(x =>
                    x.OnGuild &&
                    x.Id != request.ExceptId &&
                    x.Fraction == request.Fraction);

            return _mapper.Map<UserDto>(entity);
        }
    }
}