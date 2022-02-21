using System.Collections.Generic;
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
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Premium.Queries;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using MediatR;
using static Discord.Emote;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    [RequireCommandChannel]
    public class UserPremium : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public UserPremium(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "премиум",
            "Узнай о премиум возможностях и управляй своей подпиской")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var premium30daysPrice = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.PremiumPrice30days));
            var premium365daysPrice = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.PremiumPrice365days));
            var premium365daysFullPrice = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.PremiumFullPrice365days));

            var premiumString = $"У тебя нет активной подписки на статус {emotes.GetEmote("Premium")} премиум";

            if (user.IsPremium)
            {
                var userPremium = await _mediator.Send(new GetUserPremiumQuery(user.Id));

                premiumString =
                    $"Твоя подписка на статус {emotes.GetEmote("Premium")} премиум истекает " +
                    $"**{userPremium.Expiration.Humanize(culture: new CultureInfo("ru-RU"))}**";
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Премиум", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"тут отображается информация о статусе {emotes.GetEmote("Premium")} премиум:" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Стоимость подписки",
                    $"{emotes.GetEmote("Premium")} премиум на 30 дней стоит {emotes.GetEmote(Currency.Lobbs.ToString())} {premium30daysPrice} {_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), premium30daysPrice)}" +
                    $"\n{emotes.GetEmote("Premium")} премиум на 365 дней стоит {emotes.GetEmote(Currency.Lobbs.ToString())} ~~{premium365daysFullPrice}~~ {premium365daysPrice} {_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), premium365daysPrice)}" +
                    $"\n{emotes.GetEmote("Arrow")} Экономия {emotes.GetEmote(Currency.Lobbs.ToString())} {premium365daysFullPrice - premium365daysPrice} {_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), premium365daysFullPrice - premium365daysPrice)}")
                .AddField(StringExtensions.EmptyChar, premiumString)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.GetPremium)));

            var components = new ComponentBuilder
            {
                ActionRows = new List<ActionRowBuilder>
                {
                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                                "Узнать о преимуществах премиум статуса",
                                "user-premium-info:1",
                                ButtonStyle.Secondary,
                                emote: Parse(emotes.GetEmote("DiscordHelp")))
                            .Build()),

                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                                (user.IsPremium ? "Продлить" : "Приобрести") + " премиум на 30 дней",
                                "user-premium-buy:30",
                                emote: Parse(emotes.GetEmote("Premium")))
                            .Build())
                        .AddComponent(new ButtonBuilder(
                                (user.IsPremium ? "Продлить" : "Приобрести") + " премиум на 365 дней",
                                "user-premium-buy:365",
                                emote: Parse(emotes.GetEmote("Premium")))
                            .Build())
                }
            };

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build()));
        }
    }
}