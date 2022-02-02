using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Game.Relationship.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Relationship.Queries
{
    public record GetUserRelationshipQuery(long UserId) : IRequest<RelationshipDto>;

    public class GetUserRelationshipHandler : IRequestHandler<GetUserRelationshipQuery, RelationshipDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserRelationshipHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<RelationshipDto> Handle(GetUserRelationshipQuery request, CancellationToken ct)
        {
            var entity = await _db.Relationships
                .Include(x => x.User1)
                .Include(x => x.User2)
                .SingleOrDefaultAsync(x =>
                    x.User1Id == request.UserId ||
                    x.User2Id == request.UserId);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have relationship");
            }

            return _mapper.Map<RelationshipDto>(entity);
        }
    }
}