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
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Farm
{
    [RequireLocation(Location.Neutral)]
    public class FarmWater : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public FarmWater(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("farm-water")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userFarms = await _mediator.Send(new GetUserFarmsQuery(user.Id));

            if (userFarms.Any() is false)
            {
                throw new GameUserExpectedException(
                    $"у тебя нет {emotes.GetEmote(Building.Farm.ToString())} фермы чтобы поливать на ней семена.");
            }

            var farmToWater = userFarms.Count(x => x.State == FieldState.Planted);

            if (farmToWater < 1)
            {
                throw new GameUserExpectedException(
                    $"на твоей {emotes.GetEmote(Building.Farm.ToString())} ферме нет клеток которые нуждаются в поливке.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты отправляешься поливать свою {emotes.GetEmote(Building.Farm.ToString())} ферму." +
                    "\n\nНажми на кнопку **Бросить кубики** чтобы определить длительность поливки фермы." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Длительность",
                    "В ожидании броска кубиков")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            var components = new ComponentBuilder()
                .WithButton("Бросить кубики", "cube-drop-farm-water");

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = components.Build();
            });
        }
    }
}