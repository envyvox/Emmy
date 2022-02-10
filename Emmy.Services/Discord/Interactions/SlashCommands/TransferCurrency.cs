using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    public class TransferCurrency : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public TransferCurrency(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "передать",
            "Передать валюту указанному пользователю (с комиссией)")]
        public async Task TransferCurrencyTask(
            [Summary("пользователь", "Пользователь, которому ты хочешь передать валюту")]
            IUser mentionedUser,
            [Summary("тип-валюты", "Тип валюты которую ты хочешь передать пользователю")]
            [Choice("Токены", 1), Choice("Лоббсы", 2)]
            int currencyHashcode,
            [Summary("количество", "Количество валюты которую ты хочешь передать пользователю")] [MinValue(10)]
            uint amount)
        {
            await Context.Interaction.DeferAsync();

            if (Context.User.Id == mentionedUser.Id)
            {
                throw new GameUserExpectedException(
                    "нельзя передать валюту самому себе.");
            }

            if (mentionedUser.IsBot)
            {
                throw new GameUserExpectedException(
                    "нельзя передать валюту боту");
            }

            var currency = (Currency) currencyHashcode;

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, currency));

            if (userCurrency.Amount < amount)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(currency.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), 5)} " +
                    "для передачи указаной суммы пользователю.");
            }

            var targetUser = await _mediator.Send(new GetUserQuery((long) mentionedUser.Id));
            var targetSocketUser = await _mediator.Send(new GetSocketGuildUserQuery(mentionedUser.Id));

            if (targetSocketUser is null)
            {
                throw new GameUserExpectedException(
                    "я не смогла найти указанного пользователя на сервере. " +
                    "Возможно это ошибка со стороны дискорда и нужно попробовать позже.");
            }

            var taxPercentWithPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.TransferCurrencyTaxPercentWithPremium));
            var taxPercentWithoutPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.TransferCurrencyTaxPercentWithoutPremium));

            var amountAfterTax = (uint) (currency is Currency.Token
                ? user.IsPremium
                    ? amount - amount / 100.0 * taxPercentWithPremium
                    : amount - amount / 100.0 * taxPercentWithoutPremium
                : amount);

            await _mediator.Send(new RemoveCurrencyFromUserCommand(user.Id, currency, amount));
            await _mediator.Send(new AddCurrencyToUserCommand(targetUser.Id, currency, amountAfterTax));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Передача валюты")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно передал {targetSocketUser.Mention.AsGameMention(user.Title)} " +
                    $"{emotes.GetEmote(currency.ToString())} {amountAfterTax} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), amountAfterTax)}. " +
                    (amount - amountAfterTax > 0
                        ? $"Комиссия составила {emotes.GetEmote(currency.ToString())} {amount - amountAfterTax} " +
                          $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), amount - amountAfterTax)}."
                        : ""));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));

            var notify = new EmbedBuilder()
                .WithUserColor(targetUser.CommandColor)
                .WithAuthor("Передача валюты")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)} передал тебе " +
                    $"{emotes.GetEmote(currency.ToString())} {amountAfterTax} " +
                    $"{_local.Localize(LocalizationCategory.Currency, currency.ToString(), amountAfterTax)}.");

            await _mediator.Send(new SendEmbedToUserCommand(targetSocketUser.Id, notify));
        }
    }
}