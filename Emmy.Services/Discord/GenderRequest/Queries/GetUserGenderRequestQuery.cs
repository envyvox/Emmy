using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Discord.GenderRequest.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.GenderRequest.Queries
{
    public record GetUserGenderRequestQuery(long UserId) : IRequest<UserGenderRequestDto>;

    public class GetUserGenderRequestHandler : IRequestHandler<GetUserGenderRequestQuery, UserGenderRequestDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserGenderRequestHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<UserGenderRequestDto> Handle(GetUserGenderRequestQuery request, CancellationToken ct)
        {
            var entity = await _db.UserGenderRequests
                .SingleOrDefaultAsync(x => x.UserId == request.UserId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have gender request entity");
            }

            return _mapper.Map<UserGenderRequestDto>(entity);
        }
    }
}