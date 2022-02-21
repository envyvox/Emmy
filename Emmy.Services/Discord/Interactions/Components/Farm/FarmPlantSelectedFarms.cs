using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Farm.Commands;
using Emmy.Services.Game.Farm.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Seed.Commands;
using Emmy.Services.Game.Seed.Queries;
using Emmy.Services.Game.Statistic.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Farm
{
    [RequireLocation(Location.Neutral)]
    public class FarmPlantSelectedFarms : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public FarmPlantSelectedFarms(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("farm-plant-selected-farms:*")]
        public async Task Execute(string seedIdString, string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var seedId = Guid.Parse(seedIdString);
            var selectedFarms = selectedValues.Select(uint.Parse).ToArray();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var seed = await _mediator.Send(new GetSeedQuery(seedId));
            var userSeed = await _mediator.Send(new GetUserSeedQuery(user.Id, seed.Id));

            if (userSeed.Amount < 1)
            {
                throw new GameUserExpectedException(
                    $"у тебя нет в наличии {emotes.GetEmote(seed.Name)} " +
                    $"{_local.Localize(LocalizationCategory.Seed, seed.Name, 5)} чтобы посадить их на свою " +
                    $"{emotes.GetEmote(Building.Farm.ToString())} ферму.");
            }

            var userFarms = await _mediator.Send(new GetUserFarmsQuery(user.Id));

            userFarms = userFarms
                .Where(x => selectedFarms.Contains(x.Number))
                .ToList();

            if (userFarms.Any(x => x.State is not FieldState.Empty))
            {
                throw new GameUserExpectedException(
                    $"на выбранной ячейке {emotes.GetEmote(Building.Farm.ToString())} фермы уже посажены семена.");
            }

            await _mediator.Send(new RemoveSeedFromUserCommand(user.Id, seed.Id, (uint) selectedFarms.Length));
            await _mediator.Send(new PlantUserFarmsCommand(user.Id, selectedFarms, seed.Id));
            await _mediator.Send(new AddStatisticToUserCommand(
                user.Id, Statistic.SeedPlanted, (uint) selectedFarms.Length));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно посадил {emotes.GetEmote(seed.Name)} " +
                    $"{_local.Localize(LocalizationCategory.Seed, seed.Name, 5)} на выбранные " +
                    $"клетки своей {emotes.GetEmote(Building.Farm.ToString())} фермы.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().Build();
            });
        }
    }
}