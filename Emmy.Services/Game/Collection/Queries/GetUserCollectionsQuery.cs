using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Game.Collection.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Collection.Queries
{
    public record GetUserCollectionsQuery(long UserId, CollectionCategory Category) : IRequest<List<UserCollectionDto>>;

    public class GetUserCollectionsHandler : IRequestHandler<GetUserCollectionsQuery, List<UserCollectionDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserCollectionsHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<List<UserCollectionDto>> Handle(GetUserCollectionsQuery request, CancellationToken ct)
        {
            var entities = await _db.UserCollections
                .AsQueryable()
                .Where(x =>
                    x.UserId == request.UserId &&
                    x.Category == request.Category)
                .ToListAsync();

            return _mapper.Map<List<UserCollectionDto>>(entities);
        }
    }
}