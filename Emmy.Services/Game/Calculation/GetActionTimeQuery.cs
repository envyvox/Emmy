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
                {9, request.Duration.TotalMinutes + request.Duration.TotalMinutes * 30 / 100},
                {15, request.Duration.TotalMinutes + request.Duration.TotalMinutes * 15 / 100},
                {21, request.Duration.TotalMinutes},
                {27, request.Duration.TotalMinutes - request.Duration.TotalMinutes * 15 / 100},
                {33, request.Duration.TotalMinutes - request.Duration.TotalMinutes * 30 / 100},
                {39, request.Duration.TotalMinutes - request.Duration.TotalMinutes * 45 / 100}
            };

            return await Task.FromResult(TimeSpan.FromMinutes(dict[dict.Keys.Where(x => x <= request.CubeDrop).Max()]));
        }
    }
}