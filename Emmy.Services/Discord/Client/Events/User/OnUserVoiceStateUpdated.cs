using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Voice.Commands;
using Emmy.Services.Extensions;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.User
{
    public record OnUserVoiceStateUpdated(
            SocketUser SocketUser,
            SocketVoiceState OldSocketVoiceState,
            SocketVoiceState NewSocketVoiceState)
        : IRequest;

    public class OnUserVoiceStateUpdatedHandler : IRequestHandler<OnUserVoiceStateUpdated>
    {
        private readonly IMediator _mediator;

        public OnUserVoiceStateUpdatedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(OnUserVoiceStateUpdated request, CancellationToken ct)
        {
            var channels = DiscordRepository.Channels;

            var createRoomParent = channels[Channel.CreateRoomParent].Id;
            var createRoom = channels[Channel.CreateRoom].Id;
            var familyParent = channels[Channel.PrivateRoomParent].Id;
            var loveRoomParent = channels[Channel.LoveRoomParent].Id;
            var afkRoom = channels[Channel.Afk].Id;

            var oldChannel = request.OldSocketVoiceState.VoiceChannel;
            var newChannel = request.NewSocketVoiceState.VoiceChannel;

            if (oldChannel is null && newChannel.Id != afkRoom)
            {
                await _mediator.Send(new AddRoleToGuildUserByRoleTypeCommand(
                    request.SocketUser.Id, Data.Enums.Discord.Role.InVoice));
                await _mediator.Send(new CreateUserVoiceCommand(
                    (long) request.SocketUser.Id, (long) newChannel.Id));
            }

            if (newChannel is null || newChannel.Id == afkRoom)
            {
                await _mediator.Send(new RemoveRoleFromGuildUserByRoleTypeCommand(
                    request.SocketUser.Id, Data.Enums.Discord.Role.InVoice));
                await _mediator.Send(new DeleteUserVoiceCommand(
                    (long) request.SocketUser.Id));
            }

            if (newChannel?.Id == createRoom)
            {
                var createdChannel = await newChannel.Guild.CreateVoiceChannelAsync(request.SocketUser.Username, x =>
                {
                    x.CategoryId = createRoomParent;
                    x.UserLimit = 5;
                });

                await newChannel.Guild
                    .GetUser(request.SocketUser.Id)
                    .ModifyAsync(x => x.Channel = createdChannel);

                await createdChannel.AddPermissionOverwriteAsync(request.SocketUser,
                    new OverwritePermissions(manageChannel: PermValue.Allow));
            }

            if (newChannel?.CategoryId == familyParent &&
                newChannel.Users.Count == 1)
            {
                await newChannel.AddPermissionOverwriteAsync(newChannel.Guild.EveryoneRole,
                    new OverwritePermissions(
                        createInstantInvite: PermValue.Deny,
                        manageChannel: PermValue.Deny,
                        manageRoles: PermValue.Deny,
                        viewChannel: PermValue.Allow,
                        connect: PermValue.Deny,
                        speak: PermValue.Allow,
                        muteMembers: PermValue.Deny,
                        deafenMembers: PermValue.Deny,
                        moveMembers: PermValue.Deny,
                        useVoiceActivation: PermValue.Allow,
                        prioritySpeaker: PermValue.Deny,
                        stream: PermValue.Allow,
                        startEmbeddedActivities: PermValue.Allow));
            }

            if (newChannel?.CategoryId == loveRoomParent &&
                newChannel.Users.Count == 1)
            {
                await newChannel.AddPermissionOverwriteAsync(newChannel.Guild.EveryoneRole,
                    new OverwritePermissions(
                        createInstantInvite: PermValue.Deny,
                        manageChannel: PermValue.Deny,
                        manageRoles: PermValue.Deny,
                        viewChannel: PermValue.Allow,
                        connect: PermValue.Deny,
                        speak: PermValue.Allow,
                        useVoiceActivation: PermValue.Allow,
                        stream: PermValue.Allow,
                        startEmbeddedActivities: PermValue.Allow));
            }

            if (oldChannel?.CategoryId == createRoomParent &&
                oldChannel.Users.Count == 0 &&
                oldChannel.Id != createRoom)
            {
                await oldChannel.DeleteAsync();
            }


            if (oldChannel?.CategoryId == familyParent &&
                oldChannel.Users.Count == 0)
            {
                await oldChannel.AddPermissionOverwriteAsync(oldChannel.Guild.EveryoneRole,
                    new OverwritePermissions(
                        createInstantInvite: PermValue.Deny,
                        manageChannel: PermValue.Deny,
                        manageRoles: PermValue.Deny,
                        viewChannel: PermValue.Deny,
                        connect: PermValue.Deny,
                        speak: PermValue.Allow,
                        muteMembers: PermValue.Deny,
                        deafenMembers: PermValue.Deny,
                        moveMembers: PermValue.Deny,
                        useVoiceActivation: PermValue.Allow,
                        prioritySpeaker: PermValue.Deny,
                        stream: PermValue.Allow,
                        startEmbeddedActivities: PermValue.Allow));
            }

            if (oldChannel?.CategoryId == loveRoomParent &&
                oldChannel.Users.Count == 0)
            {
                await oldChannel.AddPermissionOverwriteAsync(oldChannel.Guild.EveryoneRole,
                    new OverwritePermissions(
                        createInstantInvite: PermValue.Deny,
                        manageChannel: PermValue.Deny,
                        manageRoles: PermValue.Deny,
                        viewChannel: PermValue.Deny,
                        connect: PermValue.Deny,
                        speak: PermValue.Allow,
                        useVoiceActivation: PermValue.Allow,
                        stream: PermValue.Allow,
                        startEmbeddedActivities: PermValue.Allow));
            }

            return Unit.Value;
        }
    }
}