using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
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

namespace Emmy.Services.Discord.Interactions.Components.Shop
{
    [RequireLocation(Location.Neutral)]
    public class ShopRoleSwitchCurrency : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopRoleSwitchCurrency(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("shop-role-select-currency:*")]
        public async Task ShopRoleSwitchCurrencyTask(string currencyHashcode)
        {
            await Context.Interaction.DeferAsync(true);

            var currency = (Currency) int.Parse(currencyHashcode);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var guild = await _mediator.Send(new GetSocketGuildQuery());
            var shopRoles = await _mediator.Send(new GetShopRolesQuery());

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин ролей", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются доступные для приобретения серверные роли:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для приобретения роли, **выбери ее** из списка под этим сообщением." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField(
                    $"Текущая валюта для оплаты {emotes.GetEmote("Arrow")} {emotes.GetEmote(currency.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString())}",
                    StringExtensions.EmptyChar)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopRole)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Оплата токенами", $"shop-role-select-currency:{Currency.Token.GetHashCode()}",
                    ButtonStyle.Primary, Parse(emotes.GetEmote(Currency.Token.ToString())),
                    disabled: currency is Currency.Token)
                .WithButton(
                    "Оплата лоббсами", $"shop-role-select-currency:{Currency.Lobbs.GetHashCode()}",
                    ButtonStyle.Primary, Parse(emotes.GetEmote(Currency.Lobbs.ToString())),
                    disabled: currency is Currency.Lobbs);

            var selectMenu = new SelectMenuBuilder()
                .WithCustomId($"shop-role-buy-currency:{currency.GetHashCode()}")
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

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = components.WithSelectMenu(selectMenu).Build();
            });
        }
    }
}