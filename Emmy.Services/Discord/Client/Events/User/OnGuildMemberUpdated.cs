using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Emmy.Data.Enums;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Banner.Commands;
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.User
{
    public record OnGuildMemberUpdated(
            Cacheable<SocketGuildUser, ulong> OldSocketGuildUser,
            SocketGuildUser NewSocketGuildUser)
        : IRequest;

    public class OnUserUpdatedHandler : IRequestHandler<OnGuildMemberUpdated>
    {
        private readonly IMediator _mediator;

        public OnUserUpdatedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(OnGuildMemberUpdated request, CancellationToken ct)
        {
            var roles = DiscordRepository.Roles;
            var boostRoleId = roles[Data.Enums.Discord.Role.NitroBoost].Id;
            var user = await _mediator.Send(new GetUserQuery((long) request.NewSocketGuildUser.Id));
            var banners = await _mediator.Send(new GetBannersQuery());
            var banner = banners.Single(x => x.Name == "Nitro Boost");

            var addedBoostRole =
                request.NewSocketGuildUser.Roles.Any(x => x.Id == boostRoleId) is true &&
                request.OldSocketGuildUser.Value.Roles.Any(x => x.Id == boostRoleId) is false;

            var removedBoostRole =
                request.NewSocketGuildUser.Roles.Any(x => x.Id == boostRoleId) is false &&
                request.OldSocketGuildUser.Value.Roles.Any(x => x.Id == boostRoleId) is true;

            if (addedBoostRole)
            {
                await _mediator.Send(new UpdateUserCubeTypeCommand(
                    user.Id, user.IsPremium ? CubeType.D12 : CubeType.D8));
                await _mediator.Send(new AddBannerToUserCommand(user.Id, banner.Id));
            }

            if (removedBoostRole)
            {
                await _mediator.Send(new UpdateUserCubeTypeCommand(
                    user.Id, user.IsPremium ? CubeType.D8 : CubeType.D6));
                await _mediator.Send(new DeleteUserBannerCommand(user.Id, banner.Id));
            }

            return Unit.Value;
        }
    }
}