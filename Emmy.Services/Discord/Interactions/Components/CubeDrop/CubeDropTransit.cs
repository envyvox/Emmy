using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Calculation;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Transit.Commands;
using Emmy.Services.Game.Transit.Queries;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Hangfire.BackgroundJobs.CompleteTransit;
using Emmy.Services.Hangfire.Commands;
using Hangfire;
using Humanizer;
using MediatR;
using static Discord.Emote;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.Components.CubeDrop
{
    public class CubeDropTransit : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public CubeDropTransit(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("cube-drop-transit:*")]
        public async Task Execute(string selectedLocationHashcode)
        {
            await Context.Interaction.DeferAsync();

            var selectedLocation = (Location) int.Parse(selectedLocationHashcode);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCurrency = await _mediator.Send(new GetUserCurrencyQuery(user.Id, Currency.Token));
            var transit = await _mediator.Send(new GetTransitQuery(user.Location, selectedLocation));

            if (userCurrency.Amount < transit.Price)
            {
                throw new ExceptionExtensions.GameUserExpectedException(
                    $"у тебя недостаточно {emotes.GetEmote(Currency.Token.ToString())} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString())} " +
                    "для оплаты этого отправления.");
            }

            var drop1 = user.CubeType.DropCube();
            var drop2 = user.CubeType.DropCube();
            var drop3 = user.CubeType.DropCube();
            var drop1CubeEmote = emotes.GetEmote(user.CubeType.EmoteName(drop1));
            var drop2CubeEmote = emotes.GetEmote(user.CubeType.EmoteName(drop2));
            var drop3CubeEmote = emotes.GetEmote(user.CubeType.EmoteName(drop3));
            var cubeDrop = drop1 + drop2 + drop3;
            var duration = await _mediator.Send(new GetActionTimeQuery(transit.Duration, cubeDrop));

            await _mediator.Send(new UpdateUserLocationCommand(user.Id, Location.InTransit));
            await _mediator.Send(new CreateUserMovementCommand(
                user.Id, transit.Departure, transit.Destination, duration));

            var jobId = BackgroundJob.Schedule<ICompleteTransitJob>(
                x => x.Execute(user.Id, transit.Destination),
                duration);

            await _mediator.Send(new CreateUserHangfireJobCommand(user.Id, HangfireJobType.Transit, jobId, duration));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Отправления")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты решил отправиться в **{transit.Destination.Localize()}**." +
                    $"\n\nНа {drop1CubeEmote} {drop2CubeEmote} {drop3CubeEmote} кубиках выпало **{drop1 + drop2 + drop3}**!" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Стоимость",
                    $"{emotes.GetEmote(Currency.Token.ToString())} {transit.Price} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), transit.Price)}",
                    true)
                .AddField("Длительность",
                    duration.Humanize(2, new CultureInfo("ru-RU")),
                    true)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Transit)));
            
            var components = new ComponentBuilder()
                .WithButton(
                    "Узнать как работают кубики",
                    "cube-drop-how-works",
                    ButtonStyle.Secondary,
                    Parse(emotes.GetEmote("DiscordHelp")));
            
            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = components.Build();
            });
        }
    }
}