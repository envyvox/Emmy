using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Discord.Role.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using Humanizer;
using MediatR;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    [RequireCommandChannel]
    public class UserRoles : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserRoles(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "роли",
            "Управляй своими ролями на сервере")]
        public async Task UserRolesTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var guild = await _mediator.Send(new GetSocketGuildQuery());
            var guildUser = await _mediator.Send(new GetSocketGuildUserQuery(Context.User.Id));
            var userRoles = await _mediator.Send(new GetUserRolesQuery(user.Id));

            var deactivateRolesMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери роли которые хочешь снять")
                .WithCustomId("user-roles-deactivate");

            var activateRolesMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери роли которые хочешь надеть")
                .WithCustomId("user-roles-activate");

            var activeRolesString = string.Empty;
            var unActiveRolesString = string.Empty;

            var activeRolesCounter = 0;
            var unActiveRolesCounter = 0;

            foreach (var role in guildUser.Roles
                .Where(x =>
                    x.IsEveryone is false &&
                    RoleHelper.GameRoles()
                        .Select(role => role.Name())
                        .Contains(x.Name) is false)
                .OrderByDescending(x => x.Position))
            {
                if (userRoles.Any(x => x.RoleId == (long) role.Id))
                {
                    var userRole = userRoles.Single(x => x.RoleId == (long) role.Id);

                    activeRolesString +=
                        $"{emotes.GetEmote("Arrow")} {role.Mention}" +
                        (userRole.Expiration is null
                            ? "\n"
                            : $", изчезнет {userRole.Expiration.Humanize(culture: new CultureInfo("ru-RU"))}\n");

                    deactivateRolesMenu.AddOption(role.Name.ToLower(), $"{role.Id}");
                    activeRolesCounter++;
                }
                else
                {
                    activeRolesString += $"{emotes.GetEmote("Locked")} {role.Mention}\n";
                }
            }


            foreach (var userRole in userRoles
                .Where(userRole => guildUser.Roles.All(x => x.Id != (ulong) userRole.RoleId)))
            {
                var guildRole = guild.Roles.First(x => x.Id == (ulong) userRole.RoleId);

                unActiveRolesString +=
                    $"{emotes.GetEmote("Arrow")} {userRole.RoleId.ToMention(MentionType.Role)}" +
                    (userRole.Expiration is null
                        ? "\n"
                        : $", изчезнет {userRole.Expiration.Humanize(culture: new CultureInfo("ru-RU"))}\n");

                activateRolesMenu.AddOption(guildRole.Name.ToLower(), $"{guildRole.Id}");
                unActiveRolesCounter++;
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Роли", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются твои серверные роли:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для того чтобы **надеть** и/или **снять** роли, " +
                    "воспользуйся списками под этим сообщением." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Активные роли",
                    activeRolesString.Length > 0
                        ? activeRolesString
                        : "У тебя нет активных ролей")
                .AddField("Неактивные роли",
                    unActiveRolesString.Length > 0
                        ? unActiveRolesString
                        : "У тебя нет неактивных ролей")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserRoles)));

            var components = new ComponentBuilder();

            if (deactivateRolesMenu.Options.Any())
            {
                components.WithSelectMenu(deactivateRolesMenu.WithMaxValues(activeRolesCounter));
            }

            if (activateRolesMenu.Options.Any())
            {
                components.WithSelectMenu(activateRolesMenu.WithMaxValues(unActiveRolesCounter));
            }


            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build()));
        }
    }
}