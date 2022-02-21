using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Humanizer;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Calculation;
using Emmy.Services.Game.Transit.Commands;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Emmy.Services.Hangfire.BackgroundJobs.CompleteFishing;
using Emmy.Services.Hangfire.Commands;
using Hangfire;
using MediatR;
using static Discord.Emote;
using static Emmy.Services.Extensions.ExceptionExtensions;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.Components.CubeDrop
{
    [RequireLocation(Location.Neutral)]
    public class CubeDropFishing : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public CubeDropFishing(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("cube-drop-fishing:*")]
        public async Task Execute(string userIdString)
        {
            await Context.Interaction.DeferAsync();

            var userId = ulong.Parse(userIdString);

            if (Context.User.Id != userId)
            {
                throw new GameUserExpectedException(
                    "эта кнопка не для тебя!");
            }

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var drop1 = user.CubeType.DropCube();
            var drop2 = user.CubeType.DropCube();
            var drop3 = user.CubeType.DropCube();
            var drop1CubeEmote = emotes.GetEmote(user.CubeType.EmoteName(drop1));
            var drop2CubeEmote = emotes.GetEmote(user.CubeType.EmoteName(drop2));
            var drop3CubeEmote = emotes.GetEmote(user.CubeType.EmoteName(drop3));
            var cubeDrop = drop1 + drop2 + drop3;

            var fishingTime = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.FishingDefaultDurationInMinutes));
            var duration = await _mediator.Send(new GetActionTimeQuery(
                TimeSpan.FromMinutes(fishingTime), cubeDrop));

            await _mediator.Send(new UpdateUserLocationCommand(user.Id, Location.Fishing));
            await _mediator.Send(new CreateUserMovementCommand(user.Id, Location.Fishing, Location.Neutral, duration));

            var jobId = BackgroundJob.Schedule<ICompleteFishingJob>(
                x => x.Execute(user.Id, cubeDrop),
                duration);

            await _mediator.Send(new CreateUserHangfireJobCommand(user.Id, HangfireJobType.Fishing, jobId, duration));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Рыбалка", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"**{Location.Neutral.Localize()}** полна желающих поймать крутой улов и теперь ты один из них. " +
                    "В надежде что богиня фортуны пошлет тебе улов потяжелее ты отправляешься на рыбалку, " +
                    "но даже самые опытные рыбаки не могут знать заранее насколько удачно все пройдет." +
                    $"\n\nНа {drop1CubeEmote} {drop2CubeEmote} {drop3CubeEmote} кубиках выпало **{drop1 + drop2 + drop3}**!" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Ожидаемая награда",
                    $"{emotes.GetEmote("Xp")} опыт и {emotes.GetEmote("OctopusBW")} случайная рыба")
                .AddField("Длительность",
                    duration.Humanize(2, new CultureInfo("ru-RU")))
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Fishing)));

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