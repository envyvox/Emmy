using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Services.Game.World.Queries;
using MediatR;

namespace Emmy.Services.Game.Fish.Queries
{
    public record GetRandomFishRarityQuery : IRequest<FishRarity>;

    public class GetRandomFishRarityHandler : IRequestHandler<GetRandomFishRarityQuery, FishRarity>
    {
        private readonly IMediator _mediator;

        private readonly Random _random = new();

        public GetRandomFishRarityHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

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
                    var chance = await _mediator.Send(new GetWorldPropertyValueQuery(rarity switch
                    {
                        FishRarity.Common => WorldProperty.FishRarityChanceCommon,
                        FishRarity.Rare => WorldProperty.FishRarityChanceRare,
                        FishRarity.Epic => WorldProperty.FishRarityChanceEpic,
                        FishRarity.Mythical => WorldProperty.FishRarityChanceMythical,
                        FishRarity.Legendary => WorldProperty.FishRarityChanceLegendary,
                        _ => throw new ArgumentOutOfRangeException()
                    }));

                    if (current <= rand && rand < current + chance) return rarity;

                    current += chance;
                }
            }
        }
    }
}