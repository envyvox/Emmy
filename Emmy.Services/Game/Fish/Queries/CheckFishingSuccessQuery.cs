using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Services.Game.World.Queries;
using MediatR;

namespace Emmy.Services.Game.Fish.Queries
{
    public record CheckFishingSuccessQuery(FishRarity Rarity) : IRequest<bool>;

    public class CheckFishingSuccessHandler : IRequestHandler<CheckFishingSuccessQuery, bool>
    {
        private readonly IMediator _mediator;

        private readonly Random _random = new();

        public CheckFishingSuccessHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> Handle(CheckFishingSuccessQuery request, CancellationToken ct)
        {
            var chance = await _mediator.Send(new GetWorldPropertyValueQuery(request.Rarity switch
            {
                FishRarity.Common => WorldProperty.FishFailChanceCommon,
                FishRarity.Rare => WorldProperty.FishFailChanceRare,
                FishRarity.Epic => WorldProperty.FishFailChanceEpic,
                FishRarity.Mythical => WorldProperty.FishFailChanceMythical,
                FishRarity.Legendary => WorldProperty.FishFailChanceLegendary,
                _ => throw new ArgumentOutOfRangeException()
            }));

            return _random.Next(1, 101) > chance;
        }
    }
}