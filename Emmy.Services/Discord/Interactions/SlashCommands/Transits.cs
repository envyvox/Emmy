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
using Emmy.Services.Game.Transit.Queries;
using Emmy.Services.Game.User.Queries;
using Humanizer;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    public class Transits : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public Transits(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "отправления",
            "Просматривай и отправляйся в доступные локации быстро и дешево")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasMovement = await _mediator.Send(new CheckUserHasMovementQuery(user.Id));

            if (hasMovement)
            {
                throw new GameUserExpectedException(
                    "просматривать или совершать отправления можно лишь когда ты ничем не занят.");
            }

            var transits = await _mediator.Send(new GetTransitsFromLocationQuery(user.Location));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Отправления")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}," +
                    "тут отображаются доступные отправления из твоей текущей локации:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Чтобы отправиться, **выбери нужную локацию** из списка под этим сообщением." +
                    $"\n{StringExtensions.EmptyChar}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Transit)))
                .WithFooter("Точная длительность отправления определяется броском кубиков.");

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери локацию в которую хочешь отправиться")
                .WithCustomId("transit-select");

            foreach (var transit in transits)
            {
                embed.AddField(
                    $"{transit.Destination.Localize()}",
                    $"Стоимость: {emotes.GetEmote(Currency.Token.ToString())} {transit.Price} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), transit.Price)}" +
                    $"\nДлительность: в среднем ~{transit.Duration.Humanize(culture: new CultureInfo("ru-RU"))}");

                selectMenu.AddOption(
                    $"Отправиться в {transit.Destination.Localize()}",
                    $"{transit.Destination.GetHashCode()}");
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed,
                new ComponentBuilder().WithSelectMenu(selectMenu).Build()));
        }
    }
}