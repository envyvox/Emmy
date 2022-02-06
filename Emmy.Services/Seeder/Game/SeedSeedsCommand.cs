using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Emmy.Services.Game.Seed.Commands;
using MediatR;

namespace Emmy.Services.Seeder.Game
{
    public record SeedSeedsCommand : IRequest<TotalAndAffectedCountDto>;

    public class SeedSeedsHandler : IRequestHandler<SeedSeedsCommand, TotalAndAffectedCountDto>
    {
        private readonly IMediator _mediator;

        public SeedSeedsHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TotalAndAffectedCountDto> Handle(SeedSeedsCommand request, CancellationToken ct)
        {
            var result = new TotalAndAffectedCountDto();
            var commands = new CreateSeedCommand[]
            {
                new("GreenBeanSeeds", Season.Spring, 3, 2, false, 60),
                new("PotatoSeeds", Season.Spring, 3, 0, true, 50),
                new("StrawberrySeeds", Season.Spring, 3, 2, true, 30),
                new("KaleSeeds", Season.Spring, 3, 0, false, 70),
                new("ParsnipSeeds", Season.Spring, 3, 0, false, 20),
                new("RhubarbSeeds", Season.Spring, 4, 0, false, 100),
                new("CauliflowerSeeds", Season.Spring, 3, 0, false, 80),
                new("GarlicSeeds", Season.Spring, 3, 0, false, 40),
                new("MelonSeeds", Season.Summer, 4, 0, false, 80),
                new("HotPepperSeeds", Season.Summer, 3, 2, true, 40),
                new("RedCabbageSeeds", Season.Summer, 4, 0, false, 100),
                new("CornSeeds", Season.Summer, 4, 2, false, 150),
                new("TomatoSeeds", Season.Summer, 3, 2, true, 50),
                new("WheatSeeds", Season.Summer, 3, 0, false, 10),
                new("RadishSeeds", Season.Summer, 4, 0, false, 40),
                new("HopsSeeds", Season.Summer, 3, 1, false, 60),
                new("BlueberrySeeds", Season.Summer, 4, 2, true, 80),
                new("AmaranthSeeds", Season.Autumn, 3, 0, false, 70),
                new("ArtichokeSeeds", Season.Autumn, 3, 0, false, 30),
                new("EggplantSeeds", Season.Autumn, 2, 3, false, 20),
                new("YamSeeds", Season.Autumn, 3, 0, true, 60),
                new("BokChoySeeds", Season.Autumn, 3, 0, false, 50),
                new("GrapeSeeds", Season.Autumn, 3, 2, false, 60),
                new("CranberrySeeds", Season.Autumn, 3, 0, true, 240),
                new("BeetSeeds", Season.Autumn, 3, 0, false, 20),
                new("PumpkinSeeds", Season.Autumn, 3, 0, false, 100),
                new("SunflowerSeeds", Season.Summer, 2, 0, true, 50),
                new("RiceSeeds", Season.Spring, 2, 0, false, 75),
                new("CoffeeBeanSeeds", Season.Spring, 4, 2, true, 700)
            };

            foreach (var command in commands)
            {
                result.Total++;

                try
                {
                    await _mediator.Send(command);

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