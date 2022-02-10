using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Discord.LoveRoom.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Relationship.Queries;
using Emmy.Services.Game.User.Queries;
using Humanizer;
using MediatR;
using static Discord.Emote;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    [RequireCommandChannel]
    public class UserRelationship : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserRelationship(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "отношения",
            "Просматривай информацию и управляй своими отношениями")]
        public async Task UserRelationshipTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasRelationship = await _mediator.Send(new CheckUserHasRelationshipQuery(user.Id));

            string relationshipString;
            string loveRoomString;

            if (hasRelationship)
            {
                var relationship = await _mediator.Send(new GetUserRelationshipQuery(user.Id));
                var partner = relationship.User1.Id == user.Id ? relationship.User2 : relationship.User1;
                var socketPartner = await _mediator.Send(new GetSocketGuildUserQuery((ulong) partner.Id));
                var isStarted = relationship.User1.Id == user.Id;

                relationshipString =
                    $"Ты в отношениях с {socketPartner.Mention.AsGameMention(partner.Title)}, " +
                    (isStarted
                        ? $"ты {(user.Gender is Gender.Male ? "предложил" : "предложила")} {(partner.Gender is Gender.Male ? "ему" : "ей")} "
                        : $"{(partner.Gender is Gender.Male ? "он предложил" : "она предложила")} тебе ") +
                    $"начать отношения {relationship.CreatedAt.Humanize(culture: new CultureInfo("ru-RU"))}";

                var hasLoveRoom = await _mediator.Send(new CheckRelationshipHasLoveRoomQuery(relationship.Id));

                if (hasLoveRoom)
                {
                    var loveRoom = await _mediator.Send(new GetLoveRoomQuery(relationship.Id));

                    loveRoomString =
                        $"{loveRoom.ChannelId.ToMention(MentionType.Channel)}, исчезнет " +
                        $"{loveRoom.Expiration.Humanize(culture: new CultureInfo("ru-RU"))}";
                }
                else
                {
                    loveRoomString =
                        "У вашей пары еще нет любовного гнезда, чтобы это исправить, **выбери из меню** под этим " +
                        $"сообщением диалог {emotes.GetEmote("DiscordHelp")} «{Context.Client.CurrentUser.Username}, как мне создать любовное гнездо?»";
                }
            }
            else
            {
                relationshipString =
                    "Ты не состоишь в отношениях." +
                    $"\n{emotes.GetEmote("Arrow")} Для того чтобы это исправить, **выбери из меню** под этим " +
                    $"сообщением диалог {emotes.GetEmote("DiscordHelp")} «{Context.Client.CurrentUser.Username}, как мне начать отношения?»";
                loveRoomString =
                    "Ты не состоишь в отношениях, соответственно у тебя не может быть любовного гнезда";
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Отношения")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображается информация о твоих отношениях:" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Информация об отношениях",
                    relationshipString)
                .AddField("Информация об любовном гнезде",
                    loveRoomString)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

            var components = new ComponentBuilder()
                .WithSelectMenu(new SelectMenuBuilder()
                    .WithPlaceholder("Выбери вопрос который тебя интересует")
                    .WithCustomId("relationship-qa")
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, как мне начать отношения?",
                        "relationship-start",
                        emote: Parse(emotes.GetEmote("DiscordHelp")))
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, как мне закончить отношения?",
                        "relationship-end",
                        emote: Parse(emotes.GetEmote("DiscordHelp")))
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, как мне создать любовное гнездо?",
                        "relationship-love-room-create",
                        emote: Parse(emotes.GetEmote("DiscordHelp")))
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, как мне продлить любовное гнездо?",
                        "relationship-love-room-update",
                        emote: Parse(emotes.GetEmote("DiscordHelp"))));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build()));
        }
    }
}