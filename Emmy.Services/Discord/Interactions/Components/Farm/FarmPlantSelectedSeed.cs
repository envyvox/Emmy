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
using Emmy.Services.Game.Farm.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Seed.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Farm
{
    [RequireLocation(Location.Neutral)]
    public class FarmPlantSelectedSeed : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public FarmPlantSelectedSeed(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("farm-plant-selected-seed")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync(true);

            var seedId = Guid.Parse(selectedValues.First());

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
            var emptyFarms = userFarms
                .Where(x => x.State is FieldState.Empty)
                .ToList();

            if (emptyFarms.Any() is false)
            {
                throw new GameUserExpectedException(
                    $"на твоей {emotes.GetEmote(Building.Farm.ToString())} ферме нет пустых клеток, " +
                    $"на которые ты смог бы посадить {emotes.GetEmote(seed.Name)} " +
                    $"{_local.Localize(LocalizationCategory.Seed, seed.Name)}.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"теперь необходимо выбрать клетки на которые ты хочешь посадить {emotes.GetEmote(seed.Name)} {_local.Localize(LocalizationCategory.Seed, seed.Name, 5)}:" +
                    $"\n{StringExtensions.EmptyChar}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери клетки фермы на которые ты хочешь посадить семена")
                .WithCustomId($"farm-plant-selected-farms:{seed.Id}")
                .WithMaxValues(userSeed.Amount >= emptyFarms.Count ? emptyFarms.Count : (int) userSeed.Amount);

            foreach (var userFarm in emptyFarms)
            {
                selectMenu.AddOption(
                    $"Клетка фермы #{userFarm.Number}",
                    $"{userFarm.Number}",
                    emote: Parse(emotes.GetEmote(Building.Farm.ToString())));
            }

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().WithSelectMenu(selectMenu).Build();
            });
        }
    }
}