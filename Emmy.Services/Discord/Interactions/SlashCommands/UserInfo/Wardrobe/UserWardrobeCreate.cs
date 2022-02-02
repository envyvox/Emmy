using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Discord.Role.Commands;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo.Wardrobe
{
    [RequirePremium]
    public class UserWardrobeCreate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserWardrobeCreate(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "гардеробная-создать",
            "Создай свою собственную роль благодаря гардеробной")]
        public async Task Execute(
            [Summary("название", "Название твоей новой собственной роли")]
            string roleName,
            [Summary("цвет", "HEX цвет твоей новой собственной роли")]
            string roleColor)
        {
            await Context.Interaction.DeferAsync();

            if (roleName.Length > 100)
            {
                throw new GameUserExpectedException(
                    "название роли не может быть длинее 100 символов.");
            }

            roleColor = roleColor.Replace("#", "");

            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var createdRoleId = await _mediator.Send(new CreateGuildPersonalRoleCommand(roleName, roleColor));
            await _mediator.Send(new AddRoleToUserCommand(user.Id, (long) createdRoleId, null, true));
            await _mediator.Send(new AddRoleToGuildUserByRoleIdCommand(Context.User.Id, createdRoleId));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Гардеробная")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно создал собственную роль {createdRoleId.ToMention(MentionType.Role)}. " +
                    "Она была добавлена в твои `/роли` а так же автоматически надета на сервере.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Wardrobe)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}