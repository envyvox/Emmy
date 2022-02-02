using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Localization.Commands
{
    public record CreateLocalizationCommand(
            LocalizationCategory Category,
            string Name,
            string Single,
            string Double,
            string Multiply)
        : IRequest;

    public class CreateLocalizationHandler : IRequestHandler<CreateLocalizationCommand>
    {
        private readonly AppDbContext _db;

        public CreateLocalizationHandler(DbContextOptions options)
        {
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(CreateLocalizationCommand request, CancellationToken ct)
        {
            var exist = await _db.Localizations
                .AnyAsync(x =>
                    x.Category == request.Category &&
                    x.Name == request.Name);

            if (exist)
            {
                throw new Exception(
                    $"localization with category {request.Category.ToString()} and name {request.Name} already exist");
            }

            await _db.CreateEntity(new Data.Entities.Localization
            {
                Id = Guid.NewGuid(),
                Category = request.Category,
                Name = request.Name,
                Single = request.Single,
                Double = request.Double,
                Multiply = request.Multiply
            });

            return Unit.Value;
        }
    }
}