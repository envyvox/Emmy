using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Emmy.Services.Game.Fish.Commands;
using MediatR;

namespace Emmy.Services.Seeder.Game
{
    public record SeedFishesCommand : IRequest<TotalAndAffectedCountDto>;

    public class SeedFishesHandler : IRequestHandler<SeedFishesCommand, TotalAndAffectedCountDto>
    {
        private readonly IMediator _mediator;

        public SeedFishesHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TotalAndAffectedCountDto> Handle(SeedFishesCommand request, CancellationToken ct)
        {
            var result = new TotalAndAffectedCountDto();
            var commands = new CreateFishCommand[]
            {
                new("Carp", FishRarity.Common, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 20),
                new("Bream", FishRarity.Common, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Night, 23),
                new("Ghostfish", FishRarity.Common, new List<Season> { Season.Summer, Season.Autumn }, Weather.Any, TimesDayType.Any, 20),
                new("Chub", FishRarity.Common, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 20),
                new("Sandfish", FishRarity.Rare, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Day, 45),
                new("Bullhead", FishRarity.Rare, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 40),
                new("Woodskip", FishRarity.Rare, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 40),
                new("LargemouthBass", FishRarity.Epic, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Day, 90),
                new("Slimejack", FishRarity.Epic, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 80),
                new("ScorpionCarp", FishRarity.Epic, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Day, 90),
                new("VoidSalmon", FishRarity.Epic, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 80),
                new("Stonefish", FishRarity.Mythical, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 160),
                new("IcePip", FishRarity.Mythical, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 160),
                new("LavaEel", FishRarity.Mythical, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 160),
                new("MutantCarp", FishRarity.Legendary, new List<Season> { Season.Any }, Weather.Any, TimesDayType.Any, 320),
                new("Sunfish", FishRarity.Common, new List<Season> { Season.Spring, Season.Summer }, Weather.Clear, TimesDayType.Day, 38),
                new("Catfish", FishRarity.Mythical, new List<Season> { Season.Spring, Season.Autumn, Season.Winter }, Weather.Rain, TimesDayType.Any, 230),
                new("Herring", FishRarity.Common, new List<Season> { Season.Spring, Season.Winter }, Weather.Any, TimesDayType.Any, 30),
                new("MidnightCarp", FishRarity.Epic, new List<Season> { Season.Autumn, Season.Winter }, Weather.Any, TimesDayType.Day, 130),
                new("Salmon", FishRarity.Rare, new List<Season> { Season.Autumn }, Weather.Any, TimesDayType.Day, 75),
                new("Sardine", FishRarity.Common, new List<Season> { Season.Spring, Season.Autumn, Season.Winter }, Weather.Any, TimesDayType.Day, 28),
                new("SmallmouthBass", FishRarity.Common, new List<Season> { Season.Spring, Season.Autumn }, Weather.Any, TimesDayType.Any, 30),
                new("Tilapia", FishRarity.Rare, new List<Season> { Season.Summer, Season.Autumn }, Weather.Any, TimesDayType.Day, 65),
                new("RedMullet", FishRarity.Rare, new List<Season> { Season.Summer, Season.Winter }, Weather.Any, TimesDayType.Day, 65),
                new("Pike", FishRarity.Epic, new List<Season> { Season.Summer, Season.Winter }, Weather.Any, TimesDayType.Any, 120),
                new("Putterfish", FishRarity.Mythical, new List<Season> { Season.Summer }, Weather.Clear, TimesDayType.Day, 312),
                new("Octopus", FishRarity.Epic, new List<Season> { Season.Summer }, Weather.Any, TimesDayType.Day, 150),
                new("RainbowTrout", FishRarity.Rare, new List<Season> { Season.Summer }, Weather.Clear, TimesDayType.Day, 85),
                new("Eel", FishRarity.Rare, new List<Season> { Season.Spring, Season.Autumn }, Weather.Rain, TimesDayType.Night, 75),
                new("Crimsonfish", FishRarity.Legendary, new List<Season> { Season.Summer }, Weather.Any, TimesDayType.Any, 520),
                new("Squid", FishRarity.Rare, new List<Season> { Season.Winter }, Weather.Any, TimesDayType.Night, 75),
                new("TigerTrout", FishRarity.Epic, new List<Season> { Season.Autumn, Season.Winter }, Weather.Any, TimesDayType.Day, 130),
                new("Halibut", FishRarity.Rare, new List<Season> { Season.Spring, Season.Summer, Season.Winter }, Weather.Any, TimesDayType.Day, 55),
                new("SeaCucumber", FishRarity.Rare, new List<Season> { Season.Autumn, Season.Winter }, Weather.Any, TimesDayType.Day, 65),
                new("Lingcod", FishRarity.Epic, new List<Season> { Season.Winter }, Weather.Any, TimesDayType.Any, 140),
                new("SuperCucumber", FishRarity.Mythical, new List<Season> { Season.Summer, Season.Autumn }, Weather.Any, TimesDayType.Night, 252),
                new("Legend", FishRarity.Legendary, new List<Season> { Season.Spring }, Weather.Rain, TimesDayType.Any, 590),
                new("Dorado", FishRarity.Epic, new List<Season> { Season.Summer }, Weather.Any, TimesDayType.Day, 150),
                new("Tuna", FishRarity.Epic, new List<Season> { Season.Summer, Season.Winter }, Weather.Any, TimesDayType.Day, 130),
                new("Glacierfish", FishRarity.Legendary, new List<Season> { Season.Winter }, Weather.Any, TimesDayType.Any, 520),
                new("Flounder", FishRarity.Epic, new List<Season> { Season.Spring, Season.Summer }, Weather.Any, TimesDayType.Day, 130),
                new("Angler", FishRarity.Legendary, new List<Season> { Season.Autumn }, Weather.Any, TimesDayType.Any, 520),
                new("Sturgeon", FishRarity.Mythical, new List<Season> { Season.Summer, Season.Winter }, Weather.Any, TimesDayType.Day, 252),
                new("Albacore", FishRarity.Rare, new List<Season> { Season.Autumn, Season.Winter }, Weather.Any, TimesDayType.Day, 65),
                new("RedSnapper", FishRarity.Common, new List<Season> { Season.Summer, Season.Autumn }, Weather.Rain, TimesDayType.Day, 38),
                new("Anchovy", FishRarity.Common, new List<Season> { Season.Spring, Season.Autumn }, Weather.Any, TimesDayType.Any, 30),
                new("Walleye", FishRarity.Epic, new List<Season> { Season.Autumn }, Weather.Rain, TimesDayType.Night, 170),
                new("Perch", FishRarity.Rare, new List<Season> { Season.Winter }, Weather.Any, TimesDayType.Any, 70),
                new("Shad", FishRarity.Rare, new List<Season> { Season.Spring, Season.Summer, Season.Autumn }, Weather.Rain, TimesDayType.Night, 65)
            };

            foreach (var createFishCommand in commands)
            {
                result.Total++;

                try
                {
                    await _mediator.Send(createFishCommand);

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