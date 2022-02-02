using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Key.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.SlashCommands.Shop
{
    [RequireLocation(Location.Neutral)]
    public class ShopKey : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public ShopKey(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "магазин-ключей",
            "Приобретай ключи открывающие различный функционал")]
        public async Task ShopKeyTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var keys = await _mediator.Send(new GetKeysQuery());

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Магазин ключей")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются доступные для приобретения ключи открывающие различный функционал:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для приобретения ключа, **выбери его** из списка под этим сообщением." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField(
                    $"Текущая валюта для оплаты {emotes.GetEmote("Arrow")} {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString())}",
                    StringExtensions.EmptyChar)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ShopKey)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Оплата токенами", $"shop-key-select-currency:{Currency.Token.GetHashCode()}",
                    ButtonStyle.Primary, Parse(emotes.GetEmote(Currency.Token.ToString())), disabled: true)
                .WithButton(
                    "Оплата лоббсами", $"shop-key-select-currency:{Currency.Lobbs.GetHashCode()}",
                    ButtonStyle.Primary, Parse(emotes.GetEmote(Currency.Lobbs.ToString())));

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери ключ который хочешь приобрести")
                .WithCustomId($"shop-key-buy-currency:{Currency.Token.GetHashCode()}");

            foreach (var key in keys)
            {
                embed.AddField(
                    $"{emotes.GetEmote(key.Type.EmoteName())} {_local.Localize(LocalizationCategory.Key, key.Type.ToString())}",
                    $"{emotes.GetEmote("Arrow")} {key.Type.Description()}" +
                    $"\nСтоимость: {emotes.GetEmote(Currency.Token.ToString())} {key.Price} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), key.Price)} или " +
                    $"{emotes.GetEmote(Currency.Lobbs.ToString())} {key.Price.ConvertTokensToLobbs()} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), key.Price.ConvertTokensToLobbs())}");

                selectMenu.AddOption(
                    _local.Localize(LocalizationCategory.Key, key.Type.ToString()),
                    $"{key.Type.GetHashCode()}",
                    key.Type.Description(),
                    Parse(emotes.GetEmote(key.Type.EmoteName())));
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed,
                components
                    .WithSelectMenu(selectMenu)
                    .Build()));
        }
    }
}