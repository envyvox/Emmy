using System;
using System.Threading;
using System.Threading.Tasks;
using CoordinateSharp;
using Emmy.Data.Enums;
using MediatR;

namespace Emmy.Services.Game.World.Queries
{
    public record GetCurrentTimesDayQuery : IRequest<TimesDayType>;

    public class GetCurrentTimesDayHandler : IRequestHandler<GetCurrentTimesDayQuery, TimesDayType>
    {
        private readonly TimeZoneInfo _timeZoneInfo;

        public GetCurrentTimesDayHandler(TimeZoneInfo timeZoneInfo)
        {
            _timeZoneInfo = timeZoneInfo;
        }

        public async Task<TimesDayType> Handle(GetCurrentTimesDayQuery request, CancellationToken ct)
        {
            var timeNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _timeZoneInfo);
            var coordinate = new Coordinate(55.915379, 37.824598, timeNow);

            return await Task.FromResult(
                timeNow > coordinate.CelestialInfo.SunRise &&
                timeNow < coordinate.CelestialInfo.SunSet
                    ? TimesDayType.Day
                    : TimesDayType.Night);
        }
    }
}