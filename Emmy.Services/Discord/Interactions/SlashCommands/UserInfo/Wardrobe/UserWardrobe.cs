using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Discord.Role.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo.Wardrobe
{
    [RequirePremium]
    public class UserWardrobe : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserWardrobe(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "гарберобная",
            "Создавай и управляй своей собственной ролью")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasPersonalRole = await _mediator.Send(new CheckUserHasPersonalRoleQuery(user.Id));

            var roleInfoString = "Ты еще не создал собственную роль";

            if (hasPersonalRole)
            {
                var personalRole = await _mediator.Send(new GetUserPersonalRoleQuery(user.Id));

                roleInfoString = $"{personalRole.RoleId.ToMention(MentionType.Role)}";
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Гардеробная")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображается информация о твоей собственной роли:")
                .AddField("Информация о роли",
                    roleInfoString)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Wardrobe)));

            var components = new ComponentBuilder()
                .WithSelectMenu(new SelectMenuBuilder()
                    .WithPlaceholder("Выбери вопрос который тебя интересует")
                    .WithCustomId("user-wardrobe-qa")
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, как мне создать собственную роль?",
                        "user-wardrobe-create")
                    .AddOption(
                        $"{Context.Client.CurrentUser.Username}, как мне обновить собственную роль?",
                        "user-wardrobe-update"))
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components));
        }
    }
}