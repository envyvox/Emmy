using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Services.Extensions;
using MediatR;

namespace Emmy.Services.Discord.Guild.Queries
{
    public record CheckGuildUserHasRoleByTypeQuery(ulong UserId, Data.Enums.Discord.Role Role) : IRequest<bool>;

    public class CheckGuildUserHasRoleByTypeHandler : IRequestHandler<CheckGuildUserHasRoleByTypeQuery, bool>
    {
        private readonly IMediator _mediator;

        public CheckGuildUserHasRoleByTypeHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> Handle(CheckGuildUserHasRoleByTypeQuery request, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetSocketGuildUserQuery(request.UserId));
            var roles = DiscordRepository.Roles;

            return user.Roles.Any(x => x.Id == roles[request.Role].Id);
        }
    }
}