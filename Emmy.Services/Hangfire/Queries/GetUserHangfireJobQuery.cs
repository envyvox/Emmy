using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Services.Hangfire.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Hangfire.Queries
{
    public record GetUserHangfireJobQuery(long UserId, HangfireJobType Type) : IRequest<UserHangfireJobDto>;

    public class GetUserHangfireJobHandler : IRequestHandler<GetUserHangfireJobQuery, UserHangfireJobDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public GetUserHangfireJobHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<UserHangfireJobDto> Handle(GetUserHangfireJobQuery request, CancellationToken ct)
        {
            var entity = await _db.UserHangfireJobs
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                throw new Exception(
                    $"User {request.UserId} doesnt have hangfire job with type {request.Type.ToString()}");
            }

            return _mapper.Map<UserHangfireJobDto>(entity);
        }
    }
}