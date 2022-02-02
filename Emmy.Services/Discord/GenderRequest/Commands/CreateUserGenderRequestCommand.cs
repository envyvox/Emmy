using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Entities.Discord;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.GenderRequest.Commands
{
    public record CreateUserGenderRequestCommand(long UserId) : IRequest;

    public class CreateUserGenderRequestHandler : IRequestHandler<CreateUserGenderRequestCommand>
    {
        private readonly ILogger<CreateUserGenderRequestHandler> _logger;
        private readonly AppDbContext _db;

        public CreateUserGenderRequestHandler(
            DbContextOptions options,
            ILogger<CreateUserGenderRequestHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateUserGenderRequestCommand request, CancellationToken ct)
        {
            var exist = await _db.UserGenderRequests
                .AnyAsync(x => x.UserId == request.UserId);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already have gender request");
            }

            var created = await _db.CreateEntity(new UserGenderRequest
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                State = RequestState.Pending,
                ModeratorId = null,
                CreatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created user gender request entity {@Entity}",
                created);

            return Unit.Value;
        }
    }
}