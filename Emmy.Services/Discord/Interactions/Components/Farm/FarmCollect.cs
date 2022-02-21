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
using Emmy.Services.Game.Collection.Commands;
using Emmy.Services.Game.Crop.Commands;
using Emmy.Services.Game.Farm.Commands;
using Emmy.Services.Game.Farm.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Statistic.Commands;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Farm
{
    [RequireLocation(Location.Neutral)]
    public class FarmCollect : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        private readonly Random _random = new();

        public FarmCollect(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("farm-collect")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userFarms = await _mediator.Send(new GetUserFarmsQuery(user.Id));

            userFarms = userFarms
                .Where(x => x.State is FieldState.Completed)
                .ToList();

            if (userFarms.Any() is false)
            {
                throw new GameUserExpectedException(
                    $"на твоей {emotes.GetEmote(Building.Farm.ToString())} ферме нет ячеек с готовым для сбора урожаем.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно собрал урожай со своей {emotes.GetEmote(Building.Farm.ToString())} фермы:" +
                    $"\n{StringExtensions.EmptyChar}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            foreach (var userFarm in userFarms)
            {
                var amount = userFarm.Seed.IsMultiply
                    ? (uint) _random.Next(2, 4)
                    : 1;

                var desc =
                    $"Ты успешно собрал {emotes.GetEmote(userFarm.Seed.Crop.Name)} {amount} " +
                    $"{_local.Localize(LocalizationCategory.Crop, userFarm.Seed.Crop.Name, amount)}.";

                if (userFarm.Seed.ReGrowthDays > 0)
                {
                    await _mediator.Send(new StartReGrowthOnUserFarmCommand(user.Id, userFarm.Number));

                    desc +=
                        $"\n{emotes.GetEmote("Arrow")} Новый урожай через " +
                        $"{userFarm.Seed.ReGrowthDays.Days().Humanize(1, new CultureInfo("ru-RU"))}.";
                }
                else
                {
                    await _mediator.Send(new ResetUserFarmCommand(user.Id, userFarm.Number));

                    desc +=
                        $"\n{emotes.GetEmote("Arrow")} Ячейка {emotes.GetEmote(Building.Farm.ToString())} фермы теперь свободна.";
                }

                var xpAmount = await _mediator.Send(new GetWorldPropertyValueQuery(
                    WorldProperty.XpCropHarvesting));

                await _mediator.Send(new AddXpToUserCommand(user.Id, xpAmount));
                await _mediator.Send(new AddCropToUserCommand(user.Id, userFarm.Seed.Crop.Id, amount));
                await _mediator.Send(new AddCollectionToUserCommand(
                    user.Id, CollectionCategory.Crop, userFarm.Seed.Crop.Id));

                embed.AddField(
                    $"{emotes.GetEmote("List")} Ячейка {emotes.GetEmote(Building.Farm.ToString())} фермы `#{userFarm.Number}`",
                    desc + $"\nПолучено {emotes.GetEmote("Xp")} {xpAmount} ед. опыта.");
            }

            await _mediator.Send(new AddStatisticToUserCommand(
                user.Id, Statistic.CropHarvested, (uint) userFarms.Count));

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().Build();
            });
        }
    }
}