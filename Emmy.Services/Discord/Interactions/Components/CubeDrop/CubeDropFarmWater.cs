using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Calculation;
using Emmy.Services.Game.Farm.Queries;
using Emmy.Services.Game.Transit.Commands;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Emmy.Services.Hangfire.BackgroundJobs.CompleteFarmWatering;
using Emmy.Services.Hangfire.Commands;
using Hangfire;
using Humanizer;
using MediatR;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.Components.CubeDrop
{
    [RequireLocation(Location.Neutral)]
    public class CubeDropFarmWater : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public CubeDropFarmWater(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("cube-drop-farm-water")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userFarms = await _mediator.Send(new GetUserFarmsQuery(user.Id));
            var farmsToWater = (uint) userFarms.Count(x => x.State == FieldState.Planted);

            var drop1 = user.CubeType.DropCube();
            var drop2 = user.CubeType.DropCube();
            var drop3 = user.CubeType.DropCube();
            var drop1CubeEmote = emotes.GetEmote(user.CubeType.EmoteName(drop1));
            var drop2CubeEmote = emotes.GetEmote(user.CubeType.EmoteName(drop2));
            var drop3CubeEmote = emotes.GetEmote(user.CubeType.EmoteName(drop3));
            var cubeDrop = drop1 + drop2 + drop3;

            var wateringTime = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.FarmWateringTime));
            var duration = await _mediator.Send(new GetActionTimeQuery(
                TimeSpan.FromMinutes(wateringTime * farmsToWater), cubeDrop));

            await _mediator.Send(new UpdateUserLocationCommand(user.Id, Location.FarmWatering));
            await _mediator.Send(new CreateUserMovementCommand(
                user.Id, Location.FarmWatering, Location.Neutral, duration));

            var jobId = BackgroundJob.Schedule<ICompleteFarmWateringJob>(
                x => x.Execute(user.Id, farmsToWater),
                duration);

            await _mediator.Send(new CreateUserHangfireJobCommand(
                user.Id, HangfireJobType.FarmWatering, jobId, duration));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты отправляешься поливать свою {emotes.GetEmote(Building.Farm.ToString())} ферму." +
                    $"\n\nНа {drop1CubeEmote} {drop2CubeEmote} {drop3CubeEmote} кубиках выпало **{drop1 + drop2 + drop3}**!" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Длительность", duration.Humanize(2, new CultureInfo("ru-RU")))
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().Build();
            });
        }
    }
}