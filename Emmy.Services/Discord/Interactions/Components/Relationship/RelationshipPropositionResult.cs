using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Relationship.Commands;
using Emmy.Services.Game.Relationship.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Relationship
{
    public class RelationshipPropositionResult : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public RelationshipPropositionResult(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("relationship-proposition-result:*,*")]
        public async Task RelationshipPropositionResultTask(string isAcceptedString, string userIdString)
        {
            await Context.Interaction.DeferAsync();

            var isAccepted = bool.Parse(isAcceptedString);
            var userId = long.Parse(userIdString);

            // чтобы не путаться в команде предложения и кнопках -
            // user это пользователь который отправил предложение
            // targetUser это пользователь который получил предложение

            var targetUser = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var user = await _mediator.Send(new GetUserQuery(userId));

            var targetSocketUser = await _mediator.Send(new GetSocketGuildUserQuery(Context.User.Id));
            var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) userId));

            if (isAccepted is true)
            {
                var targetUserHasRelationship = await _mediator.Send(new CheckUserHasRelationshipQuery(targetUser.Id));

                if (targetUserHasRelationship)
                {
                    throw new GameUserExpectedException(
                        "ты уже состоишь в отношениях и не можешь принять предложение пока не разберешься со старыми.");
                }

                var userHasRelationship = await _mediator.Send(new CheckUserHasRelationshipQuery(user.Id));

                if (userHasRelationship)
                {
                    throw new GameUserExpectedException(
                        $"к сожалению {socketUser.Mention.AsGameMention(user.Title)} уже состоит в отношениях, " +
                        $"видимо ты слишком долго {(targetUser.Gender is Gender.Male ? "думал" : "думала")}.");
                }

                await _mediator.Send(new CreateRelationshipCommand(user.Id, targetUser.Id));

                var embed = new EmbedBuilder()
                    .WithUserColor(targetUser.CommandColor)
                    .WithAuthor("Отношения")
                    .WithDescription(
                        $"Ты {(targetUser.Gender is Gender.Male ? "принял" : "приняла")} предложение отношений " +
                        $"от {socketUser.Mention.AsGameMention(user.Title)}, желаю вам всех благ!")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

                await Context.Interaction.ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = embed.Build();
                    x.Components = new ComponentBuilder().Build();
                });

                var notify = new EmbedBuilder()
                    .WithUserColor(user.CommandColor)
                    .WithAuthor("Отношения")
                    .WithDescription(
                        $"{targetSocketUser.Mention.AsGameMention(targetUser.Title)} " +
                        $"{(targetUser.Gender is Gender.Male ? "принял" : "приняла")} предложенные тобою отношения, " +
                        "желаю вам всех благ!")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

                await _mediator.Send(new SendEmbedToUserCommand((ulong) userId, notify));
            }
            else
            {
                var embed = new EmbedBuilder()
                    .WithUserColor(targetUser.CommandColor)
                    .WithAuthor("Отношения")
                    .WithDescription(
                        $"Ты {(targetUser.Gender is Gender.Male ? "отказался" : "отказалась")} от предложенных отношений.")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

                await Context.Interaction.ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = embed.Build();
                    x.Components = new ComponentBuilder().Build();
                });

                var notify = new EmbedBuilder()
                    .WithUserColor(user.CommandColor)
                    .WithAuthor("Отношения")
                    .WithDescription(
                        $"{targetSocketUser.Mention.AsGameMention(targetUser.Title)} " +
                        $"{(targetUser.Gender is Gender.Male ? "отказался" : "отказалась")} от предложенных тобою отношений.")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

                await _mediator.Send(new SendEmbedToUserCommand((ulong) userId, notify));
            }
        }
    }
}