using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Emmy.Services.Discord.Guild.Queries
{
    public record CheckGuildUserHasRoleByIdQuery(ulong UserId, ulong RoleId) : IRequest<bool>;

    public class CheckGuildUserHasRoleByIdHandler : IRequestHandler<CheckGuildUserHasRoleByIdQuery, bool>
    {
        private readonly IMediator _mediator;

        public CheckGuildUserHasRoleByIdHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> Handle(CheckGuildUserHasRoleByIdQuery request, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetSocketGuildUserQuery(request.UserId));

            return user.Roles.Any(x => x.Id == request.RoleId);
        }
    }
}