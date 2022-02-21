using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Relationship.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    public class RelationshipProposition : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public RelationshipProposition(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "предложить-отношения",
            "Предложить указанному пользователю начать с тобой отношения")]
        public async Task RelationshipPropositionTask(
            [Summary("пользователь", "Пользователь, которому ты хочешь предложить отношения")]
            IUser mentionedUser)
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            if (Context.User.Id == mentionedUser.Id)
            {
                throw new GameUserExpectedException(
                    "нельзя предложить отношения самому себе.");
            }

            if (mentionedUser.IsBot)
            {
                throw new GameUserExpectedException(
                    "нельзя предложить отношения боту.");
            }

            if (user.Gender is Gender.None)
            {
                throw new GameUserExpectedException(
                    "для того чтобы предложить отношения пользователю, " +
                    $"необходимо сперва подтведить {emotes.GetEmote(Gender.None.EmoteName())} пол.");
            }

            var userHasRelationship = await _mediator.Send(new CheckUserHasRelationshipQuery(user.Id));

            if (userHasRelationship)
            {
                throw new GameUserExpectedException(
                    "ты уже состоишь в отношениях и не можешь предложить новые, пока не разберешься со старыми.");
            }

            var targetUser = await _mediator.Send(new GetUserQuery((long) mentionedUser.Id));

            if (targetUser.Gender is Gender.None)
            {
                throw new GameUserExpectedException(
                    "предложить отношения можно лишь пользователям с " +
                    $"подтвержденным {emotes.GetEmote(Gender.None.EmoteName())} полом.");
            }

            var socketTargetUser = await _mediator.Send(new GetSocketGuildUserQuery(mentionedUser.Id));
            var targetUserHasRelationship = await _mediator.Send(new CheckUserHasRelationshipQuery(targetUser.Id));

            if (targetUserHasRelationship)
            {
                throw new GameUserExpectedException(
                    $"{socketTargetUser.Mention.AsGameMention(targetUser.Title)} уже состоит в отношениях.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Отношения", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "твое предложение об отношениях успешно отправлено " +
                    $"{socketTargetUser.Mention.AsGameMention(targetUser.Title)}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));

            var notify = new EmbedBuilder()
                .WithUserColor(targetUser.CommandColor)
                .WithAuthor("Отношения", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)} предлагает тебе начать отношения, " +
                    "ты можешь **Согласиться** или **Отказаться** нажав на кнопку под этим сообщением.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Согласиться",
                    $"relationship-proposition-result:true,{Context.User.Id}",
                    emote: Parse(emotes.GetEmote("Checkmark")))
                .WithButton(
                    "Отказаться",
                    $"relationship-proposition-result:false,{Context.User.Id}",
                    emote: Parse(emotes.GetEmote("Crossmark")));

            await socketTargetUser.SendMessageAsync(embed: notify.Build(), components: components.Build());
        }
    }
}