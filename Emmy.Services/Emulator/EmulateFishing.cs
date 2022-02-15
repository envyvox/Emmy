using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Services.Emulator.Models;
using Emmy.Services.Game.Calculation;
using Emmy.Services.Game.Fish.Queries;
using MediatR;

namespace Emmy.Services.Emulator
{
    public record EmulateFishing(
            long CycleCount,
            long FishingCount,
            TimesDayType TimesDay,
            Weather Weather,
            Season Season,
            CubeType CubeType)
        : IRequest<CycleEmulateFishingResult>;

    public class EmulateFishingHandler : IRequestHandler<EmulateFishing, CycleEmulateFishingResult>
    {
        private readonly IMediator _mediator;

        public EmulateFishingHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<CycleEmulateFishingResult> Handle(EmulateFishing request, CancellationToken ct)
        {
            var cycleResult = new CycleEmulateFishingResult
            {
                Results = new List<EmulateFishingResult>()
            };

            for (var j = 0; j < request.CycleCount; j++)
            {
                var result = new EmulateFishingResult();

                for (var i = 0; i < request.FishingCount; i++)
                {
                    var drop1 = request.CubeType.DropCube();
                    var drop2 = request.CubeType.DropCube();
                    var drop3 = request.CubeType.DropCube();
                    var cubeDrop = drop1 + drop2 + drop3;

                    var rarity = await _mediator.Send(new GetRandomFishRarityQuery(cubeDrop));
                    var fish = await _mediator.Send(new GetRandomFishWithParamsQuery(rarity, request.Weather,
                        request.TimesDay, request.Season));
                    var success = await _mediator.Send(new CheckFishingSuccessQuery(fish.Rarity));

                    if (success)
                    {
                        result.TotalSuccessCount++;

                        switch (rarity)
                        {
                            case FishRarity.Common:

                                result.CommonFishSuccess++;

                                break;
                            case FishRarity.Rare:

                                result.RareFishSuccess++;

                                break;
                            case FishRarity.Epic:

                                result.EpicFishSuccess++;

                                break;
                            case FishRarity.Mythical:

                                result.MythicalFishSuccess++;

                                break;
                            case FishRarity.Legendary:

                                result.LegendaryFishSuccess++;

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        result.FinalCurrency += fish.Price;
                    }
                    else
                    {
                        result.TotalFailCount++;

                        switch (rarity)
                        {
                            case FishRarity.Common:

                                result.CommonFishFail++;

                                break;
                            case FishRarity.Rare:

                                result.RareFishFail++;

                                break;
                            case FishRarity.Epic:

                                result.EpicFishFail++;

                                break;
                            case FishRarity.Mythical:

                                result.MythicalFishFail++;

                                break;
                            case FishRarity.Legendary:

                                result.LegendaryFishFail++;

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                cycleResult.Results.Add(result);
            }

            cycleResult.AverageTotalSuccess = cycleResult.Results.Average(x => x.TotalSuccessCount);
            cycleResult.AverageTotalFail = cycleResult.Results.Average(x => x.TotalFailCount);
            cycleResult.AverageFinalCurrency = cycleResult.Results.Average(x => x.FinalCurrency);

            return cycleResult;
        }
    }
}