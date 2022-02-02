using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
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
    public class RelationshipEnd : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public RelationshipEnd(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("relationship-end")]
        public async Task RelationshipEndTask()
        {
            await Context.Interaction.DeferAsync();

            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasRelationship = await _mediator.Send(new CheckUserHasRelationshipQuery(user.Id));

            if (hasRelationship is false)
            {
                throw new GameUserExpectedException(
                    "ты не состоишь в отношениях и соответственно не можешь их закончить.");
            }

            var relationship = await _mediator.Send(new GetUserRelationshipQuery(user.Id));
            var partner = relationship.User1.Id == user.Id ? relationship.User2 : relationship.User1;
            var socketPartner = await _mediator.Send(new GetSocketGuildUserQuery((ulong) partner.Id));

            await _mediator.Send(new DeleteRelationshipCommand(user.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Отношения")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"твои отношения с {socketPartner.Mention.AsGameMention(partner.Title)} закончены.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}