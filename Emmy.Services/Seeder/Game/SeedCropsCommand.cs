using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Util;
using Emmy.Services.Game.Crop.Commands;
using Emmy.Services.Game.Seed.Queries;
using MediatR;

namespace Emmy.Services.Seeder.Game
{
    public record SeedCropsCommand : IRequest<TotalAndAffectedCountDto>;

    public class SeedCropsHandler : IRequestHandler<SeedCropsCommand, TotalAndAffectedCountDto>
    {
        private readonly IMediator _mediator;

        public SeedCropsHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TotalAndAffectedCountDto> Handle(SeedCropsCommand request, CancellationToken ct)
        {
            var result = new TotalAndAffectedCountDto();
            var seeds = await _mediator.Send(new GetSeedsQuery());

            foreach (var seed in seeds)
            {
                result.Total++;

                try
                {
                    await _mediator.Send(new CreateCropCommand(seed.Name.Replace("Seeds", ""), 999, seed.Id));

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