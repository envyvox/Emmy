using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Calculation;
using Emmy.Services.Game.Collection.Commands;
using Emmy.Services.Game.Fish.Commands;
using Emmy.Services.Game.Fish.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Statistic.Commands;
using Emmy.Services.Game.Transit.Commands;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Emmy.Services.Hangfire.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Image = Emmy.Data.Enums.Image;

namespace Emmy.Services.Hangfire.BackgroundJobs.CompleteFishing
{
    public class CompleteFishingJob : ICompleteFishingJob
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CompleteFishingJob> _logger;
        private readonly ILocalizationService _local;

        public CompleteFishingJob(
            IMediator mediator,
            ILogger<CompleteFishingJob> logger,
            ILocalizationService local)
        {
            _mediator = mediator;
            _logger = logger;
            _local = local;
        }

        public async Task Execute(long userId, uint cubeDrop)
        {
            _logger.LogInformation(
                "Complete fishing job executed for user {UserId}",
                userId);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery(userId));
            var timesDay = await _mediator.Send(new GetCurrentTimesDayQuery());
            var weather = await _mediator.Send(new GetWeatherTodayQuery());
            var season = await _mediator.Send(new GetCurrentSeasonQuery());
            var rarity = await _mediator.Send(new GetRandomFishRarityQuery(cubeDrop));
            var fish = await _mediator.Send(new GetRandomFishWithParamsQuery(rarity, weather, timesDay, season));
            var success = await _mediator.Send(new CheckFishingSuccessQuery(fish.Rarity));
            var fishingXp = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.XpFishing));

            await _mediator.Send(new UpdateUserLocationCommand(user.Id, Location.Neutral));
            await _mediator.Send(new DeleteUserMovementCommand(user.Id));
            await _mediator.Send(new DeleteUserHangfireJobCommand(user.Id, HangfireJobType.Fishing));
            await _mediator.Send(new AddXpToUserCommand(userId, fishingXp));

            var embed = new EmbedBuilder()
                .WithAuthor(Location.Fishing.Localize())
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Image.Fishing)));

            if (success)
            {
                await _mediator.Send(new AddFishToUserCommand(userId, fish.Id, 1));
                await _mediator.Send(new AddCollectionToUserCommand(userId, CollectionCategory.Fish, fish.Id));
                await _mediator.Send(new AddStatisticToUserCommand(userId, Statistic.Fishing));
                // todo check achievements

                embed
                    .WithDescription(
                        "Ты возвращаешься с улыбкой на лице и гордо демонстрируешь жителям города полученную рыбу." +
                        "\nЕсть чем гордиться, понимаю, но рыбы в здешних водах еще полно, возвращайся за новым уловом поскорее!" +
                        $"\n{StringExtensions.EmptyChar}")
                    .AddField("Полученная награда",
                        $"{emotes.GetEmote("Xp")} {fishingXp} ед. опыта и {emotes.GetEmote(fish.Name)} {_local.Localize(LocalizationCategory.Fish, fish.Name)}");
            }
            else
            {
                embed
                    .WithDescription(
                        "Сегодня явно не твой день, ведь вернувшись тебе совсем нечем похвастаться перед жителями города." +
                        $"\nТы почти поймал {emotes.GetEmote(fish.Name)} {_local.Localize(LocalizationCategory.Fish, fish.Name)}, " +
                        "однако хитрая рыба смогла сорваться с крючка. Но не расстраивайся, " +
                        "рыба в здешних водах никуда не денется, возвращайся и попытай удачу еще раз!" +
                        $"\n{StringExtensions.EmptyChar}")
                    .AddField("Полученная награда",
                        $"{emotes.GetEmote("Xp")} {fishingXp} ед. опыта");
            }

            // todo check achievements
            await _mediator.Send(new SendEmbedToUserCommand((ulong) user.Id, embed));
        }
    }
}