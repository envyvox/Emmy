using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Title.Commands
{
    public record AddTitleToUserCommand(long UserId, Data.Enums.Title Type) : IRequest;

    public class AddTitleToUserHandler : IRequestHandler<AddTitleToUserCommand>
    {
        private readonly ILogger<AddTitleToUserHandler> _logger;
        private readonly AppDbContext _db;

        public AddTitleToUserHandler(
            DbContextOptions options,
            ILogger<AddTitleToUserHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(AddTitleToUserCommand request, CancellationToken ct)
        {
            var exist = await _db.UserTitles
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Type == request.Type);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already have title {request.Type.ToString()}");
            }

            var created = await _db.CreateEntity(new UserTitle
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = request.Type,
                CreatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created user title entity {@Entity}",
                created);

            return Unit.Value;
        }
    }
}