using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Banner.Commands;
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.UserBanners
{
    public class UserBannerUpdate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserBannerUpdate(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("user-banner-update")]
        public async Task UserBannerUpdateTask(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var bannerId = Guid.Parse(selectedValues.First());

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var banner = await _mediator.Send(new GetBannerQuery(bannerId));
            var activeBanner = await _mediator.Send(new GetUserActiveBannerQuery(user.Id));

            if (banner.Id == activeBanner.Id)
            {
                throw new GameUserExpectedException(
                    $"баннер {emotes.GetEmote(banner.Rarity.EmoteName())} {banner.Rarity.Localize().ToLower()} " +
                    $"«{banner.Name}» уже отмечен как активный.");
            }

            await _mediator.Send(new DeactivateUserBannerCommand(user.Id, activeBanner.Id));
            await _mediator.Send(new ActivateUserBannerCommand(user.Id, banner.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Баннеры", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"твой баннер успешно обновлен на {emotes.GetEmote(banner.Rarity.EmoteName())} " +
                    $"{banner.Rarity.Localize().ToLower()} «{banner.Name}».")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserBanners)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}