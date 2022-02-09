using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components
{
    public class ReferralRewards : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public ReferralRewards(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("referral-rewards")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Награды реферальной системы")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ReferralRewards)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));
        }
    }
}