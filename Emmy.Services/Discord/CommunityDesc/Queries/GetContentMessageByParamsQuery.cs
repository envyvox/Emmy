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
    public record GetContentMessageByParamsQuery(long ChannelId, long MessageId) : IRequest<ContentMessageDto>;

    public class GetContentMessageByParamsHandler : IRequestHandler<GetContentMessageByParamsQuery, ContentMessageDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetContentMessageByParamsHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<ContentMessageDto> Handle(GetContentMessageByParamsQuery request, CancellationToken ct)
        {
            var entity = await _db.ContentMessages
                .Include(x => x.User)
                .SingleOrDefaultAsync(x =>
                    x.ChannelId == request.ChannelId &&
                    x.MessageId == request.MessageId);

            if (entity is null)
            {
                throw new Exception(
                    $"content message with channel {request.ChannelId} and message {request.MessageId} ids not found");
            }

            return _mapper.Map<ContentMessageDto>(entity);
        }
    }
}