using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Transit.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components
{
    public class TransitSelect : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public TransitSelect(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("transit-select")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var selectedLocation = (Location) int.Parse(selectedValues.First());

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, Currency.Token));
            var transit = await _mediator.Send(new GetTransitQuery(user.Location, selectedLocation));

            if (userCurrency.Amount < transit.Price)
            {
                throw new GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString())} " +
                    "для оплаты этого отправления.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Отправления")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты решил отправиться в **{transit.Destination.Localize()}**." +
                    "\n\nНажми на кнопку **Бросить кубики** чтобы определить успех твоего отправления." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Стоимость",
                    $"{emotes.GetEmote(Currency.Token.ToString())} {transit.Price} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), transit.Price)}",
                    true)
                .AddField("Длительность",
                    "В ожидании броска кубиков",
                    true)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Transit)));

            var components = new ComponentBuilder()
                .WithButton("Бросить кубики", $"cube-drop-transit:{selectedValues.First()}");

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = components.Build();
            });
        }
    }
}