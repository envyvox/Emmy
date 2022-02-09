using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Discord.Role.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo.Wardrobe
{
    [RequirePremium]
    public class UserWardrobeUpdate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserWardrobeUpdate(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "гардеробная-обновить",
            "Обнови свою собственную роль благодаря гардеробной")]
        public async Task Execute(
            [Summary("название", "Новое название твоей собственной роли")]
            string roleName,
            [Summary("цвет", "Новый HEX цвет твоей собственной роли")]
            string roleColor)
        {
            await Context.Interaction.DeferAsync();

            if (roleName.Length > 100)
            {
                throw new GameUserExpectedException(
                    "название роли не может быть длинее 100 символов.");
            }

            roleColor = roleColor.Replace("#", "");

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasPersonalRole = await _mediator.Send(new CheckUserHasPersonalRoleQuery(user.Id));

            if (hasPersonalRole is false)
            {
                throw new GameUserExpectedException(
                    "у тебя нет собственной роли чтобы ее обновлять. " +
                    $"Если ты хотел создать ее, то тебе необходимо написать {emotes.GetEmote("SlashCommand")} `/гардеробная-создать`.");
            }

            var personalRole = await _mediator.Send(new GetUserPersonalRoleQuery(user.Id));
            await _mediator.Send(new ModifyGuildRoleCommand((ulong) personalRole.RoleId, roleName, roleColor));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Гардеробная")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно обновил собственную роль {personalRole.RoleId.ToMention(MentionType.Role)}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Wardrobe)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}