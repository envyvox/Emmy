using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;

namespace Emmy.Services.Discord.Guild.Queries
{
    public record GetSocketRoleQuery(ulong RoleId) : IRequest<SocketRole>;

    public class GetSocketRoleHandler : IRequestHandler<GetSocketRoleQuery, SocketRole>
    {
        private readonly IMediator _mediator;

        public GetSocketRoleHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<SocketRole> Handle(GetSocketRoleQuery request, CancellationToken ct)
        {
            var guild = await _mediator.Send(new GetSocketGuildQuery());
            var role = guild.Roles.SingleOrDefault(x => x.Id == request.RoleId);

            if (role is null)
            {
                throw new Exception(
                    $"role with id {request.RoleId} not found in guild");
            }

            return role;
        }
    }
}