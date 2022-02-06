using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.Commands
{
    public record CreateUserHangfireJobCommand(
            long UserId,
            HangfireJobType Type,
            string JobId,
            TimeSpan Duration)
        : IRequest;

    public class CreateUserHangfireJobHandler : IRequestHandler<CreateUserHangfireJobCommand>
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CreateUserHangfireJobHandler> _logger;

        public CreateUserHangfireJobHandler(
            DbContextOptions options,
            ILogger<CreateUserHangfireJobHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(CreateUserHangfireJobCommand request, CancellationToken cancellationToken)
        {
            var exist = await _db.UserHangfireJobs
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already have hangfire job with type {request.Type.ToString()}");
            }

            var created = await _db.CreateEntity(new UserHangfireJob
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = request.Type,
                JobId = request.JobId,
                CreatedAt = DateTimeOffset.UtcNow,
                Expiration = DateTimeOffset.UtcNow.Add(request.Duration)
            });

            _logger.LogInformation(
                "Created user hangfire job entity {@Entity}",
                created);

            return Unit.Value;
        }
    }
}