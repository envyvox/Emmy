using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Services.Emulator.Models;
using MediatR;

namespace Emmy.Services.Emulator
{
    public record EmulateCasino(uint BetCount, uint BetAmount) : IRequest<EmulateCasinoResult>;

    public class EmulateCasinoHandler : IRequestHandler<EmulateCasino, EmulateCasinoResult>
    {
        private readonly Random _random = new();

        public async Task<EmulateCasinoResult> Handle(EmulateCasino request, CancellationToken ct)
        {
            var result = new EmulateCasinoResult
            {
                BetCount = request.BetCount,
                BetAmount = request.BetAmount
            };

            for (var i = 0; i <= request.BetCount; i++)
            {
                double firstDrop = _random.Next(1, 101);
                double secondDrop = _random.Next(1, 101);
                var cubeDrop = Math.Floor((firstDrop + secondDrop) / 2);

                switch (cubeDrop)
                {
                    case >= 55 and < 90:
                        result.WinX2++;
                        result.FinalCurrency += request.BetAmount;
                        break;
                    case >= 90 and < 100:
                        result.WinX4++;
                        result.FinalCurrency += request.BetAmount * 3;
                        break;
                    case 100:
                        result.WinX10++;
                        result.FinalCurrency += request.BetAmount * 9;
                        break;
                    default:
                        result.Lose++;
                        result.FinalCurrency -= request.BetAmount;
                        break;
                }
            }

            return await Task.FromResult(result);
        }
    }
}