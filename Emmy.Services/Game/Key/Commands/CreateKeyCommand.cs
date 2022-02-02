using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Key.Commands
{
    public record CreateKeyCommand(KeyType Type, uint Price) : IRequest;

    public class CreateKeyHandler : IRequestHandler<CreateKeyCommand>
    {
        private readonly ILogger<CreateKeyHandler> _logger;
        private readonly AppDbContext _db;

        public CreateKeyHandler(
            DbContextOptions options,
            ILogger<CreateKeyHandler> logger)
        {
            _db = new AppDbContext(options);
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateKeyCommand request, CancellationToken ct)
        {
            var exist = await _db.Keys
                .AnyAsync(x => x.Type == request.Type);

            if (exist)
            {
                throw new Exception(
                    $"key {request.Type.ToString()} already exist");
            }

            var created = await _db.CreateEntity(new Data.Entities.Key
            {
                Type = request.Type,
                Price = request.Price
            });

            _logger.LogInformation(
                "Created key entity {@Entity}",
                created);

            return Unit.Value;
        }
    }
}