using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Game.Localization.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.Localization.Commands
{
    public record UpdateLocalizationCommand(
            Guid Id,
            string Single,
            string Double,
            string Multiply)
        : IRequest<LocalizationDto>;

    public class UpdateLocalizationHandler : IRequestHandler<UpdateLocalizationCommand, LocalizationDto>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public UpdateLocalizationHandler(
            DbContextOptions options,
            IMapper mapper)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
        }

        public async Task<LocalizationDto> Handle(UpdateLocalizationCommand request, CancellationToken ct)
        {
            var entity = await _db.Localizations
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (entity is null)
            {
                throw new Exception(
                    $"localization {request.Id} not found");
            }

            entity.Single = request.Single;
            entity.Double = request.Double;
            entity.Multiply = request.Multiply;

            await _db.UpdateEntity(entity);

            return _mapper.Map<LocalizationDto>(entity);
        }
    }
}