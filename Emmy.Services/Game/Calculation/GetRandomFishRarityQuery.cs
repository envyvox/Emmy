using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using MediatR;

namespace Emmy.Services.Game.Calculation
{
    public record GetRandomFishRarityQuery(uint CubeDrop) : IRequest<FishRarity>;

    public class GetRandomFishRarityHandler : IRequestHandler<GetRandomFishRarityQuery, FishRarity>
    {
        private readonly Random _random = new();

        public async Task<FishRarity> Handle(GetRandomFishRarityQuery request, CancellationToken ct)
        {
            while (true)
            {
                var rand = _random.Next(1, 101);
                uint current = 0;

                foreach (var rarity in Enum
                    .GetValues(typeof(FishRarity))
                    .Cast<FishRarity>())
                {
                    var chanceDict = rarity switch
                    {
                        FishRarity.Common => CommonFishChances(),
                        FishRarity.Rare => RareFishChances(),
                        FishRarity.Epic => EpicFishChances(),
                        FishRarity.Mythical => MythicalFishChances(),
                        FishRarity.Legendary => LegendaryFishChances(),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    var chance = chanceDict[chanceDict.Keys.Where(x => x <= request.CubeDrop).Max()];

                    if (current <= rand && rand < current + chance) return await Task.FromResult(rarity);

                    current += chance;
                }
            }
        }

        private static Dictionary<uint, uint> CommonFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 60},
                {8, 53},
                {13, 33},
                {18, 30},
                {22, 25},
                {26, 20},
                {32, 15}
            };
        }

        private static Dictionary<uint, uint> RareFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 35},
                {8, 37},
                {13, 30},
                {18, 30},
                {22, 25},
                {26, 20},
                {32, 15}
            };
        }

        private static Dictionary<uint, uint> EpicFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 5},
                {8, 7},
                {13, 20},
                {18, 20},
                {22, 20},
                {26, 20},
                {32, 25}
            };
        }

        private static Dictionary<uint, uint> MythicalFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 0},
                {8, 3},
                {13, 12},
                {18, 10},
                {22, 20},
                {26, 25},
                {32, 25}
            };
        }

        private static Dictionary<uint, uint> LegendaryFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 0},
                {8, 0},
                {13, 5},
                {18, 10},
                {22, 10},
                {26, 15},
                {32, 20}
            };
        }
    }
}