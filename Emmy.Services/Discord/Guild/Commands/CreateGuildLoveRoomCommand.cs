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
    public record CreateGuildLoveRoomCommand(ulong User1Id, ulong User2Id) : IRequest<ulong>;

    public class CreateGuildLoveRoomHandler : IRequestHandler<CreateGuildLoveRoomCommand, ulong>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateGuildLoveRoomHandler> _logger;

        public CreateGuildLoveRoomHandler(
            IMediator mediator,
            ILogger<CreateGuildLoveRoomHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ulong> Handle(CreateGuildLoveRoomCommand request, CancellationToken ct)
        {
            var channels = DiscordRepository.Channels;
            var loveRoomParent = channels[Channel.LoveRoomParent].Id;

            var guild = await _mediator.Send(new GetSocketGuildQuery());
            var socketUser1 = guild.GetUser(request.User1Id);
            var socketUser2 = guild.GetUser(request.User2Id);

            var created = await guild.CreateVoiceChannelAsync(
                $"{socketUser1.Username} <3 {socketUser2.Username}", x =>
                {
                    x.CategoryId = loveRoomParent;
                    x.UserLimit = 2;
                });

            await created.AddPermissionOverwriteAsync(guild.EveryoneRole, new OverwritePermissions(
                createInstantInvite: PermValue.Deny,
                manageChannel: PermValue.Deny,
                manageRoles: PermValue.Deny,
                viewChannel: PermValue.Deny,
                connect: PermValue.Deny,
                speak: PermValue.Allow,
                useVoiceActivation: PermValue.Allow,
                stream: PermValue.Allow,
                startEmbeddedActivities: PermValue.Allow
            ));

            await created.AddPermissionOverwriteAsync(socketUser1, new OverwritePermissions(
                viewChannel: PermValue.Allow,
                connect: PermValue.Allow));

            await created.AddPermissionOverwriteAsync(socketUser2, new OverwritePermissions(
                viewChannel: PermValue.Allow,
                connect: PermValue.Allow));

            _logger.LogInformation(
                "Created love room for users {User1Id} and {User2Id}",
                request.User1Id, request.User2Id);

            return created.Id;
        }
    }
}