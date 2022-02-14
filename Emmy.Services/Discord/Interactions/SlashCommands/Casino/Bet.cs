using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Cooldown.Commands;
using Emmy.Services.Game.Cooldown.Queries;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Statistic.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using MediatR;
using static Discord.Emote;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.Casino
{
    [RequireCommandChannel]
    [RequireLocation(Location.Neutral)]
    public class Bet : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        private readonly Random _random = new();

        public Bet(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "ставка",
            "Сделать ставку в казино")]
        public async Task Execute(
            [Summary("количество", "Количество которое ты хочешь поставить")] [MinValue(20)] [MaxValue(200)]
            uint amount)
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            if (user.Fraction is Data.Enums.Fraction.Neutral)
            {
                throw new GameUserExpectedException(
                    "хоть это место и зовется **Нейтральной зоной**, местные не очень " +
                    $"доверяют {emotes.GetEmote(Data.Enums.Fraction.Neutral.EmoteName())} **нейтралам** и " +
                    "не хотят рисковать пуская в элитное казино неизвестно кого." +
                    "\n\nТебе необходимо заручиться поддержкой фракции, ведь даже простое упоминание их имен открывает множество дверей." +
                    $"\n\n{emotes.GetEmote("Arrow")} Чтобы вступить во фракцию, напиши {emotes.GetEmote("DiscordSlashCommand")} `/фракция`.");
            }

            var userCooldown = await _mediator.Send(new GetUserCooldownQuery(user.Id, Cooldown.CasinoBet));

            if (userCooldown.Expiration > DateTimeOffset.UtcNow)
            {
                throw new GameUserExpectedException(
                    "к сожалению, необходимо подождать немного перед следующей ставкой, она будет доступна " +
                    $"**{userCooldown.Expiration.Humanize(culture: new CultureInfo("ru-RU"))}**.");
            }

            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, Currency.Token));

            if (userCurrency.Amount < amount)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), 5)} " +
                    "для оплаты этой ставки.");
            }

            double firstDrop = _random.Next(1, 101);
            double secondDrop = _random.Next(1, 101);
            var cubeDrop = Math.Floor((firstDrop + secondDrop) / 2);

            var cubeDropString =
                $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                $"на кубиках выпадает **{cubeDrop}**.\n\n";

            switch (cubeDrop)
            {
                case >= 55 and < 90:

                    cubeDropString +=
                        "Прямо чувствуется, как повышается азарт от игры и выигранных " +
                        $"{emotes.GetEmote(Currency.Token.ToString())} {amount * 2} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), amount * 2)}! " +
                        "Главное, не теряй свое чувство меры!";

                    await _mediator.Send(new AddCurrencyToUserCommand(
                        user.Id, Currency.Token, amount));

                    break;

                case >= 90 and < 100:

                    cubeDropString +=
                        "Прямо чувствуется, как повышается азарт от игры и выигранных " +
                        $"{emotes.GetEmote(Currency.Token.ToString())} {amount * 4} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), amount * 4)}! " +
                        "Главное, не теряй свое чувство меры!";

                    await _mediator.Send(new AddCurrencyToUserCommand(
                        user.Id, Currency.Token, amount * 3));

                    break;

                case 100:

                    cubeDropString +=
                        "Прямо чувствуется, как повышается азарт от игры и выигранных " +
                        $"{emotes.GetEmote(Currency.Token.ToString())} {amount * 10} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), amount * 10)}! " +
                        "Главное, не теряй свое чувство меры!";

                    await _mediator.Send(new AddStatisticToUserCommand(
                        user.Id, Statistic.CasinoBet));
                    await _mediator.Send(new AddCurrencyToUserCommand(
                        user.Id, Currency.Token, amount * 9));

                    break;

                default:

                    cubeDropString +=
                        $"Сожалеем, ты проиграл {emotes.GetEmote(Currency.Token.ToString())} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), amount)}! " +
                        "Не сильно вини дилера.";

                    await _mediator.Send(new RemoveCurrencyFromUserCommand(
                        user.Id, Currency.Token, amount));

                    break;
            }

            var cooldownDurationInMinutes = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.CasinoBetCooldownDurationInMinutes));

            await _mediator.Send(new AddCooldownToUserCommand(
                user.Id, Cooldown.CasinoBet, TimeSpan.FromMinutes(cooldownDurationInMinutes)));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Казино")
                .WithDescription(cubeDropString)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Casino)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Узнать как работают ставки",
                    "casino-bet-how-works",
                    ButtonStyle.Secondary,
                    Parse(emotes.GetEmote("DiscordHelp")))
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components));
        }
    }
}