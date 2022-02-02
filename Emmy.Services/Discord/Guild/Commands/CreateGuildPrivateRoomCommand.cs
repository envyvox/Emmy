using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record CreateGuildPrivateRoomCommand(ulong OwnerId) : IRequest<ulong>;

    public class CreateGuildPrivateRoomHandler : IRequestHandler<CreateGuildPrivateRoomCommand, ulong>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateGuildPrivateRoomHandler> _logger;

        public CreateGuildPrivateRoomHandler(
            IMediator mediator,
            ILogger<CreateGuildPrivateRoomHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ulong> Handle(CreateGuildPrivateRoomCommand request, CancellationToken ct)
        {
            var channels = DiscordRepository.Channels;
            var privateRoomParent = channels[Channel.PrivateRoomParent].Id;

            var guild = await _mediator.Send(new GetSocketGuildQuery());
            var socketUser = guild.GetUser(request.OwnerId);

            var created =
                await guild.CreateVoiceChannelAsync("Переименуй меня", x => { x.CategoryId = privateRoomParent; });

            await created.AddPermissionOverwriteAsync(guild.EveryoneRole, new OverwritePermissions(
                createInstantInvite: PermValue.Deny,
                manageChannel: PermValue.Deny,
                manageRoles: PermValue.Deny,
                viewChannel: PermValue.Deny,
                connect: PermValue.Deny,
                speak: PermValue.Allow,
                useVoiceActivation: PermValue.Allow,
                stream: PermValue.Allow,
                startEmbeddedActivities: PermValue.Allow));

            await created.AddPermissionOverwriteAsync(socketUser, new OverwritePermissions(
                manageChannel: PermValue.Allow,
                manageRoles: PermValue.Allow,
                viewChannel: PermValue.Allow,
                connect: PermValue.Allow));

            _logger.LogInformation(
                "Created private room for user {UserId}",
                request.OwnerId);

            return created.Id;
        }
    }
}