using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.CubeDrop
{
    public class CubeDropHowWorks : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public CubeDropHowWorks(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("cube-drop-how-works")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Как работают кубики", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "после нажатия на кнопку **Бросить кубики** ты бросаешь три своих кубика и их сумма становится результатом броска кубиков и определяет твой успех." +
                    $"\n\n{emotes.GetEmote("Arrow")} По-умолчанию твои кубики это {emotes.GetEmote("CubeD61")}{emotes.GetEmote("CubeD66")} D6, что означает что ты можешь выбросить **от 3 до 18**." +
                    $"\n\n{emotes.GetEmote("Arrow")} Если ты обладаешь статусом {emotes.GetEmote("Premium")} премиум **или** поддержал сервер своим\n{emotes.GetEmote("DiscordNitroBoost")} Nitro Boost, то твои кубики превращаются в {emotes.GetEmote("CubeD81")}{emotes.GetEmote("CubeD88")} D8, что означает что ты можешь выбросить **от 3 до 24**." +
                    $"\n\n{emotes.GetEmote("Arrow")} Если ты обладаешь и статусом {emotes.GetEmote("Premium")} премиум **и** поддержал сервер своим\n{emotes.GetEmote("DiscordNitroBoost")} Nitro Boost одновременно, то твои кубики превращаются в {emotes.GetEmote("CubeD121")}{emotes.GetEmote("CubeD1212")} D12, что означает что ты можешь выбросить **от 3 до 36**.");

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));
        }
    }
}