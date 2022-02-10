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
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.ShopRole.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.SlashCommands.Shop
{
    [RequireCommandChannel]
    [RequireLocation(Location.Neutral)]
    public class ShopRole : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopRole(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "магазин-ролей",
            "Приобретай различные серверные роли")]
        public async Task ShopRoleTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var guild = await _mediator.Send(new GetSocketGuildQuery());
            var shopRoles = await _mediator.Send(new GetShopRolesQuery());

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин ролей")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются доступные для приобретения серверные роли:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для приобретения роли, **выбери ее** из списка под этим сообщением." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField(
                    $"Текущая валюта для оплаты {emotes.GetEmote("Arrow")} {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString())}",
                    StringExtensions.EmptyChar)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopRole)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Оплата токенами", $"shop-role-select-currency:{Currency.Token.GetHashCode()}",
                    ButtonStyle.Primary, Parse(emotes.GetEmote(Currency.Token.ToString())), disabled: true)
                .WithButton(
                    "Оплата лоббсами", $"shop-role-select-currency:{Currency.Lobbs.GetHashCode()}",
                    ButtonStyle.Primary, Parse(emotes.GetEmote(Currency.Lobbs.ToString())));

            var selectMenu = new SelectMenuBuilder()
                .WithCustomId($"shop-role-buy-currency:{Currency.Token.GetHashCode()}")
                .WithPlaceholder("Выбери роль которую хочешь приобрести");

            foreach (var shopRole in shopRoles)
            {
                var role = guild.GetRole((ulong) shopRole.RoleId);

                embed.AddField(StringExtensions.EmptyChar, 
                    $"Роль {role.Mention} на 30 дней" +
                    $"\nСтоимость: {emotes.GetEmote(Currency.Token.ToString())} {shopRole.Price} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), shopRole.Price)} или " +
                    $"{emotes.GetEmote(Currency.Lobbs.ToString())} {shopRole.Price.ConvertTokensToLobbs()} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), shopRole.Price.ConvertTokensToLobbs())}");

                selectMenu.AddOption(role.Name.ToLower(), $"{role.Id}");
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed,
                components
                    .WithSelectMenu(selectMenu)
                    .Build()));
        }
    }
}