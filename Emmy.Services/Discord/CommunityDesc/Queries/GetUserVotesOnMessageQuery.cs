using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Discord.CommunityDesc.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Discord.CommunityDesc.Queries
{
    public record GetUserVotesOnMessageQuery(
            long UserId,
            Guid ContentMessageId)
        : IRequest<Dictionary<Vote, ContentVoteDto>>;

    public class GetUserVotesOnMessageHandler
        : IRequestHandler<GetUserVotesOnMessageQuery, Dictionary<Vote, ContentVoteDto>>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserVotesOnMessageHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<Dictionary<Vote, ContentVoteDto>> Handle(GetUserVotesOnMessageQuery request,
            CancellationToken ct)
        {
            var entities = await _db.ContentVotes
                .Include(x => x.User)
                .Include(x => x.ContentMessage)
                .Where(x =>
                    x.UserId == request.UserId &&
                    x.ContentMessageId == request.ContentMessageId)
                .ToDictionaryAsync(x => x.Vote);

            return _mapper.Map<Dictionary<Vote, ContentVoteDto>>(entities);
        }
    }
}