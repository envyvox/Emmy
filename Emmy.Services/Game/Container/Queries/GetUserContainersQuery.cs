using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Container.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Container.Queries
{
    public record GetUserContainersQuery(long UserId) : IRequest<Dictionary<Data.Enums.Container, UserContainerDto>>;

    public class GetUserContainersHandler
        : IRequestHandler<GetUserContainersQuery, Dictionary<Data.Enums.Container, UserContainerDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserContainersHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<Dictionary<Data.Enums.Container, UserContainerDto>> Handle(GetUserContainersQuery request,
            CancellationToken ct)
        {
            var entities = await _db.UserContainers
                .AsQueryable()
                .Where(x => x.UserId == request.UserId)
                .ToDictionaryAsync(x => x.Type);

            return _mapper.Map<Dictionary<Data.Enums.Container, UserContainerDto>>(entities);
        }
    }
}