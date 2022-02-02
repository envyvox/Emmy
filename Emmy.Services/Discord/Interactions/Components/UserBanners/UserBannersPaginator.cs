using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.Components.UserBanners
{
    public class UserBannersPaginator : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserBannersPaginator(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("user-banners-paginator:*")]
        public async Task USerBannersPaginatorTask(string isForwardString)
        {
            await Context.Interaction.DeferAsync(true);

            var isForward = bool.Parse(isForwardString);
            var originalResponse = await Context.Interaction.GetOriginalResponseAsync();
            var messageEmbed = originalResponse.Embeds.First();
            var currentPage = int.Parse(Regex.Match(messageEmbed.Footer?.Text!, @"\d+").Value);
            var newPage = isForward ? currentPage + 1 : currentPage - 1;

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var activeBanner = await _mediator.Send(new GetUserActiveBannerQuery(user.Id));
            var banners = await _mediator.Send(new GetUserBannersQuery(user.Id));

            banners = banners
                .Where(x => x.IsActive is false)
                .ToList();

            var maxPages = banners.Count / 5;

            banners = banners
                .Skip(newPage > 1 ? newPage * 5 : 0)
                .Take(5)
                .ToList();

            var components = new ComponentBuilder()
                .WithButton("Назад", "user-banners-paginator:false", disabled: newPage <= 1)
                .WithButton("Вперед", "user-banners-paginator:true", disabled: newPage >= maxPages);

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
                .WithFooter($"Страница {newPage}");

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

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = components.Build();
            });
        }
    }
}