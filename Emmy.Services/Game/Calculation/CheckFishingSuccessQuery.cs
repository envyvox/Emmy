using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using MediatR;

namespace Emmy.Services.Game.Calculation
{
    public record CheckFishingSuccessQuery(FishRarity Rarity, uint CubeDrop) : IRequest<bool>;

    public class CheckFishingSuccessHandler : IRequestHandler<CheckFishingSuccessQuery, bool>
    {
        private readonly Random _random = new();

        public async Task<bool> Handle(CheckFishingSuccessQuery request, CancellationToken ct)
        {
            var chanceDict = request.Rarity switch
            {
                FishRarity.Common => CommonFishChances(),
                FishRarity.Rare => RareFishChances(),
                FishRarity.Epic => EpicFishChances(),
                FishRarity.Mythical => MythicalFishChances(),
                FishRarity.Legendary => LegendaryFishChances(),
                _ => throw new ArgumentOutOfRangeException()
            };
            var chance = chanceDict[chanceDict.Keys.Where(x => x <= request.CubeDrop).Max()];

            return await Task.FromResult(_random.Next(1, 101) > chance);
        }

        private static Dictionary<uint, uint> CommonFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 22},
                {8, 17},
                {13, 10},
                {18, 5},
                {22, 4},
                {26, 3},
                {32, 2}
            };
        }

        private static Dictionary<uint, uint> RareFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 32},
                {8, 27},
                {13, 20},
                {18, 15},
                {22, 16},
                {26, 14},
                {32, 12}
            };
        }

        private static Dictionary<uint, uint> EpicFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 40},
                {8, 35},
                {13, 30},
                {18, 20},
                {22, 26},
                {26, 24},
                {32, 22}
            };
        }

        private static Dictionary<uint, uint> MythicalFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 100},
                {8, 60},
                {13, 55},
                {18, 45},
                {22, 50},
                {26, 47},
                {32, 43}
            };
        }

        private static Dictionary<uint, uint> LegendaryFishChances()
        {
            return new Dictionary<uint, uint>
            {
                {3, 100},
                {8, 100},
                {13, 80},
                {18, 70},
                {22, 72},
                {26, 68},
                {32, 66}
            };
        }
    }
}