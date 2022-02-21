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
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Farm
{
    [RequireLocation(Location.Neutral)]
    public class FarmDig : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public FarmDig(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("farm-dig")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userFarms = await _mediator.Send(new GetUserFarmsQuery(user.Id));

            userFarms = userFarms
                .Where(x => x.State is not FieldState.Empty)
                .ToList();

            if (userFarms.Any() is false)
            {
                throw new GameUserExpectedException(
                    $"на твоей {emotes.GetEmote(Building.Farm.ToString())} ферме нет ячеек " +
                    "с которых можно выкопать семена или урожай.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"для начала необходимо **выбрать клетки** {emotes.GetEmote(Building.Farm.ToString())} " +
                    "фермы из меню под этим сообщением, с которых ты хочешь выкопать семена или урожай:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Выкапывание полностью уничтожает посаженные семяна или выращенный урожай.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери клетки с которых хочешь выкопать семена или урожай")
                .WithCustomId("farm-dig-selected")
                .WithMaxValues(userFarms.Count);

            foreach (var userFarm in userFarms)
            {
                selectMenu.AddOption(
                    $"Клетка фермы #{userFarm.Number}",
                    $"{userFarm.Number}",
                    $"Выкопать {_local.Localize(LocalizationCategory.Seed, userFarm.Seed.Name)} с клетки #{userFarm.Number}",
                    Parse(emotes.GetEmote(userFarm.Seed.Name)));
            }

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().WithSelectMenu(selectMenu).Build();
            });
        }
    }
}