using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Emmy.Services.Game.World.Commands;
using MediatR;

namespace Emmy.Services.Seeder.Game
{
    public record SeedWorldPropertiesCommand : IRequest<TotalAndAffectedCountDto>;

    public class SeedWorldPropertiesHandler : IRequestHandler<SeedWorldPropertiesCommand, TotalAndAffectedCountDto>
    {
        private readonly IMediator _mediator;

        public SeedWorldPropertiesHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TotalAndAffectedCountDto> Handle(SeedWorldPropertiesCommand request, CancellationToken ct)
        {
            var result = new TotalAndAffectedCountDto();

            foreach (var type in Enum
                .GetValues(typeof(WorldProperty))
                .Cast<WorldProperty>())
            {
                result.Total++;

                try
                {
                    await _mediator.Send(new CreateWorldPropertyCommand(type, 1));

                    result.Affected++;
                }
                catch
                {
                    // ignored
                }
            }

            return result;
        }
    }
}