using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Image.Commands
{
    public record CreateImageCommand(Data.Enums.Image Type, string Url) : IRequest;

    public class CreateImageHandler : IRequestHandler<CreateImageCommand>
    {
        private readonly ILogger<CreateImageHandler> _logger;
        private readonly AppDbContext _db;

        public CreateImageHandler(
            DbContextOptions options,
            ILogger<CreateImageHandler> logger)
        {
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(CreateImageCommand request, CancellationToken ct)
        {
            var exist = await _db.Images
                .AnyAsync(x => x.Url == request.Url);

            if (exist)
            {
                throw new Exception(
                    $"image with url {request.Url} already in db");
            }

            var created = await _db.CreateEntity(new Data.Entities.Discord.Image
            {
                Type = request.Type,
                Url = request.Url
            });

            _logger.LogInformation(
                "Created image {@Image}",
                created);

            return Unit.Value;
        }
    }
}