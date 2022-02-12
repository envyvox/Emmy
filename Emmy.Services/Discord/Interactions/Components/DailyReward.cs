using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Container.Commands;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.DailyReward.Commands;
using Emmy.Services.Game.DailyReward.Queries;
using Emmy.Services.Game.Fish.Commands;
using Emmy.Services.Game.Fish.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Seed.Commands;
using Emmy.Services.Game.Seed.Queries;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components
{
    public class DailyReward : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public DailyReward(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("daily-reward:*")]
        public async Task Execute(string userIdString)
        {
            await Context.Interaction.DeferAsync();

            var userId = ulong.Parse(userIdString);

            if (Context.User.Id != userId)
            {
                throw new GameUserExpectedException(
                    "эта кнопка не для тебя!");
            }

            var timeNow = DateTimeOffset.UtcNow;
            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasTodayReward = await _mediator.Send(new CheckUserDailyRewardQuery(
                user.Id, timeNow.DayOfWeek));


            if (hasTodayReward)
            {
                throw new GameUserExpectedException(
                    "сегодня ты уже получал ежедневную награду, попробуй заглянуть " +
                    $"**через {(timeNow.AddDays(1).Date - timeNow).Humanize(culture: new CultureInfo("ru-RU"))}**.");
            }

            await _mediator.Send(new CreateUserDailyRewardCommand(user.Id, timeNow.DayOfWeek));

            string rewardString;
            switch (timeNow.DayOfWeek)
            {
                case DayOfWeek.Monday:
                {
                    var amountWithoutPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardMondayAmountWithoutPremium));
                    var amountPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardMondayAmountPremium));
                    var amount = user.IsPremium ? amountPremium : amountWithoutPremium;

                    await _mediator.Send(new AddCurrencyToUserCommand(user.Id, Currency.Token, amount));

                    rewardString =
                        $"{emotes.GetEmote(Currency.Token.ToString())} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), amount)}";

                    break;
                }

                case DayOfWeek.Tuesday:
                {
                    var amountWithoutPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardTuesdayAmountWithoutPremium));
                    var amountPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardTuesdayAmountPremium));
                    var amount = user.IsPremium ? amountPremium : amountWithoutPremium;

                    await _mediator.Send(new AddCurrencyToUserCommand(user.Id, Currency.Lobbs, amount));

                    rewardString =
                        $"{emotes.GetEmote(Currency.Lobbs.ToString())} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), amount)}";

                    break;
                }

                case DayOfWeek.Wednesday:
                {
                    var amountWithoutPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardWednesdayAmountWithoutPremium));
                    var amountPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardWednesdayAmountPremium));
                    var amount = user.IsPremium ? amountPremium : amountWithoutPremium;

                    await _mediator.Send(new AddCurrencyToUserCommand(user.Id, Currency.Token, amount));

                    rewardString =
                        $"{emotes.GetEmote(Currency.Token.ToString())} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), amount)}";

                    break;
                }

                case DayOfWeek.Thursday:
                {
                    var amountWithoutPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardThursdayAmountWithoutPremium));
                    var amountPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardThursdayAmountPremium));
                    var amount = user.IsPremium ? amountPremium : amountWithoutPremium;

                    await _mediator.Send(new AddContainerToUserCommand(user.Id, Container.Supply, amount));

                    rewardString =
                        $"{emotes.GetEmote(Container.Supply.EmoteName())} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Container, Container.Supply.ToString(), amount)}";

                    break;
                }
                case DayOfWeek.Friday:
                {
                    var amountWithoutPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardFridayAmountWithoutPremium));
                    var amountPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardFridayAmountPremium));
                    var amount = user.IsPremium ? amountPremium : amountWithoutPremium;
                    var randomFish = await _mediator.Send(new GetRandomFishWithRarityQuery(FishRarity.Epic));

                    await _mediator.Send(new AddFishToUserCommand(user.Id, randomFish.Id, amount));

                    rewardString =
                        $"{emotes.GetEmote(randomFish.Name)} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Fish, randomFish.Name, amount)}";

                    break;
                }
                case DayOfWeek.Saturday:
                {
                    var amountWithoutPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardSaturdayAmountWithoutPremium));
                    var amountPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardSaturdayAmountPremium));
                    var amount = user.IsPremium ? amountPremium : amountWithoutPremium;
                    var randomSeed = await _mediator.Send(new GetRandomSeedQuery());

                    await _mediator.Send(new AddSeedToUserCommand(user.Id, randomSeed.Id, amount));

                    rewardString =
                        $"{emotes.GetEmote(randomSeed.Name)} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Seed, randomSeed.Name, amount)}";

                    break;
                }
                case DayOfWeek.Sunday:
                {
                    var amountWithoutPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardSundayAmountWithoutPremium));
                    var amountPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.DailyRewardSundayAmountPremium));
                    var amount = user.IsPremium ? amountPremium : amountWithoutPremium;

                    await _mediator.Send(new AddCurrencyToUserCommand(user.Id, Currency.Lobbs, amount));

                    rewardString =
                        $"{emotes.GetEmote(Currency.Lobbs.ToString())} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), amount)}";

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var userDailyRewards = await _mediator.Send(new GetUserDailyRewardsQuery(user.Id));

            if (userDailyRewards.Count is 7)
            {
                var amountWithoutPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                    WorldProperty.DailyRewardBonusAmountWithoutPremium));
                var amountPremium = await _mediator.Send(new GetWorldPropertyValueQuery(
                    WorldProperty.DailyRewardBonusAmountPremium));
                var amount = user.IsPremium ? amountPremium : amountWithoutPremium;

                await _mediator.Send(new AddCurrencyToUserCommand(user.Id, Currency.Token, amount));

                rewardString +=
                    $", а так же {emotes.GetEmote(Currency.Token.ToString())} {amount} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), amount)} " +
                    "в качестве бонуса за получение всех наград в течении недели";
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ежедневная награда")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты получаешь награду: {rewardString}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(user.IsPremium
                    ? Data.Enums.Image.DailyRewardPremium
                    : Data.Enums.Image.DailyReward)));

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().Build();
            });
        }
    }
}