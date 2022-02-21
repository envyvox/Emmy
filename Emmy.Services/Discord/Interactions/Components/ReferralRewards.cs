using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Banner.Queries;
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

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var banners = await _mediator.Send(new GetBannersQuery());
            var banner = banners.Single(x => x.Name == "Биба и Боба");

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Награды реферальной системы", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"[Нажми сюда чтобы посмотреть {emotes.GetEmote(banner.Rarity.EmoteName())} " +
                    $"{banner.Rarity.Localize().ToLower()} баннер «{banner.Name}»]({banner.Url})")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ReferralRewards)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));
        }
    }
}