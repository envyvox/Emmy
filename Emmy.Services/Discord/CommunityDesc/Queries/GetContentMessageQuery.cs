using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Discord.CommunityDesc.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.CommunityDesc.Queries
{
    public record GetContentMessageQuery(Guid Id) : IRequest<ContentMessageDto>;

    public class GetContentMessageHandler : IRequestHandler<GetContentMessageQuery, ContentMessageDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetContentMessageHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<ContentMessageDto> Handle(GetContentMessageQuery request, CancellationToken ct)
        {
            var entity = await _db.ContentMessages
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (entity is null)
            {
                throw new Exception(
                    $"content message {request.Id} not found");
            }

            return _mapper.Map<ContentMessageDto>(entity);
        }
    }
}