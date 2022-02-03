using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;

namespace Emmy.Services.Discord.Client.Events.ScheduledEvent
{
    public record OnGuildScheduledEventStarted(SocketGuildEvent SocketGuildEvent) : IRequest;

    public class OnGuildScheduledEventStartedHandler : IRequestHandler<OnGuildScheduledEventStarted>
    {
        private readonly IMediator _mediator;

        public OnGuildScheduledEventStartedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(OnGuildScheduledEventStarted request, CancellationToken ct)
        {
            var users = await request.SocketGuildEvent
                .GetUsersAsync(RequestOptions.Default)
                .FlattenAsync();
            var restUsers = users as RestUser[] ?? users.ToArray();

            if (restUsers.Any())
            {
                var emotes = DiscordRepository.Emotes;
                var channels = DiscordRepository.Channels;
                var channel = await _mediator.Send(new GetSocketTextChannelQuery(channels[Channel.EventLobby].Id));
                var usersString = restUsers.Aggregate(string.Empty, (s, v) => s + $"{v.Mention}, ");

                await channel.SendMessageAsync(
                    $"{usersString.RemoveFromEnd(2)}, мероприятие {emotes.GetEmote("DiscordScheduledEvent")} " +
                    $"**{request.SocketGuildEvent.Name}** только что началось, присоединяйтесь в {channel.Mention}!");
            }

            return Unit.Value;
        }
    }
}