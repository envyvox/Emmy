using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Discord.Role.Commands;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.ShopRole.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Shop
{
    [RequireLocation(Location.Neutral)]
    public class ShopRoleBuy : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopRoleBuy(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("shop-role-buy-currency:*")]
        public async Task ShopRoleBuyTask(string currencyHashcode, string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var currency = (Currency) int.Parse(currencyHashcode);
            var roleId = long.Parse(selectedValues.First());

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, currency));
            var shopRole = await _mediator.Send(new GetShopRoleQuery(roleId));
            var rolePrice = currency is Currency.Token ? shopRole.Price : shopRole.Price.ConvertTokensToLobbs();
            var guild = await _mediator.Send(new GetSocketGuildQuery());
            var socketRole = guild.GetRole((ulong) roleId);

            if (userCurrency.Amount < rolePrice)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(currency.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), 5)} " +
                    $"для приобретения роли {socketRole.Mention} на 30 дней.");
            }

            await _mediator.Send(new RemoveCurrencyFromUserCommand(user.Id, currency, rolePrice));
            await _mediator.Send(new AddRoleToUserCommand(user.Id, roleId, TimeSpan.FromDays(30)));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин ролей")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно приобрел роль {socketRole.Mention} на 30 дней за " +
                    $"{emotes.GetEmote(currency.ToString())} {rolePrice} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), rolePrice)}." +
                    $"\n{emotes.GetEmote("Arrow")} Найти приобретенную роль можно в {emotes.GetEmote("SlashCommand")} `/роли`.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopRole)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}