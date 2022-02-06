using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.Commands
{
    public record DeleteUserHangfireJobCommand(long UserId, HangfireJobType Type) : IRequest;

    public class DeleteUserHangfireJobHandler : IRequestHandler<DeleteUserHangfireJobCommand>
    {
        private readonly ILogger<DeleteUserHangfireJobHandler> _logger;
        private readonly AppDbContext _db;

        public DeleteUserHangfireJobHandler(
            DbContextOptions options,
            ILogger<DeleteUserHangfireJobHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(DeleteUserHangfireJobCommand request, CancellationToken ct)
        {
            var entity = await _db.UserHangfireJobs
                .SingleOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (entity is null)
            {
                throw new Exception(
                    $"user {request.UserId} doesnt have hangfire job entity with type {request.Type.ToString()}");
            }

            BackgroundJob.Delete(entity.JobId);
            await _db.DeleteEntity(entity);

            _logger.LogInformation(
                "Deleted hangfire job entity with user id {UserId} and type {Type}",
                request.UserId, request.Type.ToString());

            return Unit.Value;
        }
    }
}