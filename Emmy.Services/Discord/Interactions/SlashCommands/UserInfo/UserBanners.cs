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
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    public class UserBanners : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserBanners(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "баннеры",
            "Управляй баннером своего профиля")]
        public async Task UserBannersTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var activeBanner = await _mediator.Send(new GetUserActiveBannerQuery(user.Id));
            var banners = await _mediator.Send(new GetUserBannersQuery(user.Id));

            var components = new ComponentBuilder()
                .WithButton("Назад", "user-banners-paginator:false", disabled: true)
                .WithButton("Вперед", "user-banners-paginator:true", disabled: banners.Count <= 5);

            banners = banners
                .Where(x => x.IsActive is false)
                .Take(5)
                .ToList();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Баннеры")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображается твоя коллекция баннеров:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для того чтобы установить баннер в свой профиль, **выбери его** из списка под этим сообщением." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField(
                    $"Текущий баннер {emotes.GetEmote("Arrow")} {emotes.GetEmote(activeBanner.Rarity.EmoteName())} " +
                    $"{activeBanner.Rarity.Localize()} «{activeBanner.Name}»",
                    StringExtensions.EmptyChar)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserBanners)))
                .WithFooter("Страница 1");


            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери баннер который хочешь установить в профиль")
                .WithCustomId("user-banner-update");

            foreach (var (banner, _) in banners)
            {
                embed.AddField(
                    $"{emotes.GetEmote(banner.Rarity.EmoteName())} {banner.Rarity.Localize()} «{banner.Name}»",
                    $"[Нажми сюда чтобы посмотреть]({banner.Url})");

                selectMenu.AddOption(
                    banner.Name.ToLower(),
                    $"{banner.Id}",
                    emote: Parse(emotes.GetEmote(banner.Rarity.EmoteName())));
            }

            if (selectMenu.Options.Any())
            {
                components.WithSelectMenu(selectMenu);
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build()));
        }
    }
}