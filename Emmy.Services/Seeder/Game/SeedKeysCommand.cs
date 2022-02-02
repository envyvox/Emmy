using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Emmy.Services.Game.Key.Commands;
using MediatR;

namespace Emmy.Services.Seeder.Game
{
    public record SeedKeysCommand : IRequest<TotalAndAffectedCountDto>;

    public class SeedKeysHandler : IRequestHandler<SeedKeysCommand, TotalAndAffectedCountDto>
    {
        private readonly IMediator _mediator;

        public SeedKeysHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TotalAndAffectedCountDto> Handle(SeedKeysCommand request, CancellationToken ct)
        {
            var result = new TotalAndAffectedCountDto();

            foreach (var type in Enum
                .GetValues(typeof(KeyType))
                .Cast<KeyType>())
            {
                result.Total++;

                try
                {
                    await _mediator.Send(new CreateKeyCommand(type, 99999));

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