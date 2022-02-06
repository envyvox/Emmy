using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Emmy.Services.Game.Transit.Commands;
using MediatR;

namespace Emmy.Services.Seeder.Game
{
    public record SeedTransitsCommand : IRequest<TotalAndAffectedCountDto>;

    public class SeedTransitsHandler : IRequestHandler<SeedTransitsCommand, TotalAndAffectedCountDto>
    {
        private readonly IMediator _mediator;

        public SeedTransitsHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TotalAndAffectedCountDto> Handle(SeedTransitsCommand request, CancellationToken ct)
        {
            var result = new TotalAndAffectedCountDto();
            var locations = new List<Location>
            {
                Location.Neutral,
                Location.RedRose,
                Location.WhiteCrow,
                Location.GoldenShark
            };

            foreach (var departure in locations)
            {
                foreach (var destination in locations.Where(x => x != departure))
                {
                    result.Total++;

                    try
                    {
                        await _mediator.Send(new CreateTransitCommand(
                            departure, destination, TimeSpan.FromMinutes(5), 1));

                        result.Affected++;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            return result;
        }
    }
}