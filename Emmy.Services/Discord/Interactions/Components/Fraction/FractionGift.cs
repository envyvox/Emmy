using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Achievement.Commands;
using Emmy.Services.Game.Container.Commands;
using Emmy.Services.Game.Cooldown.Commands;
using Emmy.Services.Game.Cooldown.Queries;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Fraction.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Fraction
{
    [RequireLocation(Location.Neutral)]
    public class FractionGift : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public FractionGift(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("fraction-gift")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCooldown = await _mediator.Send(new GetUserCooldownQuery(user.Id, Cooldown.FractionGift));

            if (userCooldown.Expiration > DateTimeOffset.UtcNow)
            {
                throw new GameUserExpectedException(
                    $"ты недавно уже отправлял {emotes.GetEmote(Container.Token.EmoteName())}{emotes.GetEmote(Container.Supply.EmoteName())} " +
                    "припасы случайному пользователю своей фракции, мы не можем доставлять их так часто, " +
                    $"возвращайся к нам **{userCooldown.Expiration.Humanize(culture: new CultureInfo("ru-RU"))}**.");
            }

            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, Currency.Token));
            var giftPrice = await _mediator.Send(new GetWorldPropertyValueQuery(WorldProperty.FractionGiftPrice));

            if (userCurrency.Amount < giftPrice)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), 5)} " +
                    $"для оплаты отправки {emotes.GetEmote(Container.Token.EmoteName())}{emotes.GetEmote(Container.Supply.EmoteName())} " +
                    "припасов случайному пользователю твоей фракции.");
            }

            var giftCooldownDurationInHours = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.FractionGiftCooldownDurationInHours));
            var xpFractionGift = await _mediator.Send(new GetWorldPropertyValueQuery(WorldProperty.XpFractionGift));
            var randomUser = await _mediator.Send(new GetRandomFractionUserQuery(user.Fraction, user.Id));
            var randomSocketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) randomUser.Id));

            await _mediator.Send(new RemoveCurrencyFromUserCommand(user.Id, Currency.Token, giftPrice));
            await _mediator.Send(new AddCooldownToUserCommand(
                user.Id, Cooldown.FractionGift, TimeSpan.FromHours(giftCooldownDurationInHours)));
            await _mediator.Send(new AddXpToUserCommand(user.Id, xpFractionGift));
            await _mediator.Send(new CheckAchievementsInUserCommand(user.Id, new[]
            {
                Achievement.FirstFractionGift,
                Achievement.Fraction10Gift,
                Achievement.Fraction50Gift,
                Achievement.Fraction333Gift
            }));
            await _mediator.Send(new AddContainerToUserCommand(randomUser.Id, Container.Token, 1));
            await _mediator.Send(new AddContainerToUserCommand(randomUser.Id, Container.Supply, 1));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Фракция", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно отправил {randomSocketUser?.Mention.AsGameMention(randomUser.Title)} " +
                    $"{emotes.GetEmote(Container.Token.EmoteName())}{emotes.GetEmote(Container.Supply.EmoteName())} " +
                    $"припасы и заплатил за это {emotes.GetEmote(Currency.Token.ToString())} {giftPrice} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), giftPrice)}." +
                    $"\n\n{emotes.GetEmote("Arrow")} Получено {emotes.GetEmote("Xp")} {xpFractionGift} ед. опыта.");

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));

            var notify = new EmbedBuilder()
                .WithUserColor(randomUser.CommandColor)
                .WithAuthor("Фракция", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{randomSocketUser?.Mention.AsGameMention(randomUser.Title)}, " +
                    "тебе случайным образом достаются отправленные " +
                    $"{Context.User.Mention.AsGameMention(user.Title)} благотворительные припасы!" +
                    $"\n\nВ твой {emotes.GetEmote("DiscordSlashCommand")} `/инвентарь` были добавлены " +
                    $"{emotes.GetEmote(Container.Token.EmoteName())} 1 " +
                    $"{_local.Localize(LocalizationCategory.Container, Container.Token.ToString())} и " +
                    $"{emotes.GetEmote(Container.Supply.EmoteName())} 1 " +
                    $"{_local.Localize(LocalizationCategory.Container, Container.Supply.ToString())}.");

            await _mediator.Send(new SendEmbedToUserCommand(randomSocketUser!.Id, notify));
        }
    }
}