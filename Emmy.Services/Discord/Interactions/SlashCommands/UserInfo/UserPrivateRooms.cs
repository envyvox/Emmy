using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Discord.PrivateRoom.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using Humanizer;
using MediatR;
using static Discord.Emote;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    [RequireCommandChannel]
    public class UserPrivateRooms : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserPrivateRooms(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "приватные-секторы",
            "Просматривай информацию и управляй своими приватными секторами")]
        public async Task UserPrivateRoomsTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var familyRooms = await _mediator.Send(new GetUserPrivateRoomsQuery(user.Id));

            var channelsString = string.Empty;
            if (familyRooms.Any())
            {
                var guild = await _mediator.Send(new GetSocketGuildQuery());

                foreach (var familyRoom in familyRooms)
                {
                    var channel = guild.GetVoiceChannel((ulong) familyRoom.ChannelId);

                    channelsString +=
                        $"{channel.Mention}, исчезнет {familyRoom.Expiration.Humanize(culture: new CultureInfo("ru-RU"))}\n";
                }
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Приватные секторы")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются твои приватные секторы:" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Твои приватные секторы",
                    channelsString.Length > 0
                        ? channelsString
                        : "У тебя нет приватных секторов")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.PrivateRoom)));

            var components = new ComponentBuilder()
                .WithSelectMenu(new SelectMenuBuilder()
                    .WithPlaceholder("Выбери вопрос который тебя интересует")
                    .WithCustomId("private-room-qa")
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, как мне создать приватный сектор?",
                        "private-room-create",
                        emote: Parse(emotes.GetEmote("DiscordHelp")))
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, как мне продлить приватный сектор?",
                        "private-room-update",
                        emote: Parse(emotes.GetEmote("DiscordHelp"))));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build()));
        }
    }
}