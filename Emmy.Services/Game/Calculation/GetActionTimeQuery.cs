using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Emmy.Services.Game.Calculation
{
    public record GetActionTimeQuery(TimeSpan Duration, uint CubeDrop) : IRequest<TimeSpan>;

    public class GetActionTimeHandler : IRequestHandler<GetActionTimeQuery, TimeSpan>
    {
        public async Task<TimeSpan> Handle(GetActionTimeQuery request, CancellationToken ct)
        {
            var dict = new Dictionary<uint, double>
            {
                {3, request.Duration.TotalMinutes + request.Duration.TotalMinutes * 45 / 100},
                {8, request.Duration.TotalMinutes + request.Duration.TotalMinutes * 30 / 100},
                {13, request.Duration.TotalMinutes + request.Duration.TotalMinutes * 15 / 100},
                {18, request.Duration.TotalMinutes},
                {22, request.Duration.TotalMinutes - request.Duration.TotalMinutes * 15 / 100},
                {26, request.Duration.TotalMinutes - request.Duration.TotalMinutes * 30 / 100},
                {32, request.Duration.TotalMinutes - request.Duration.TotalMinutes * 45 / 100}
            };

            return await Task.FromResult(TimeSpan.FromMinutes(dict[dict.Keys.Where(x => x <= request.CubeDrop).Max()]));
        }
    }
}