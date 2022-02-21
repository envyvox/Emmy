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
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Farm
{
    [RequireLocation(Location.Neutral)]
    public class FarmDigSelected : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public FarmDigSelected(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("farm-dig-selected")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userFarms = await _mediator.Send(new GetUserFarmsQuery(user.Id));

            userFarms = userFarms
                .Where(x => selectedValues.Select(uint.Parse).Contains(x.Number))
                .ToList();

            if (userFarms.Any(x => x.State is FieldState.Empty))
            {
                throw new GameUserExpectedException(
                    $"на выбранной ячейке {emotes.GetEmote(Building.Farm.ToString())} фермы и так ничего не растет.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно выкопал семена или урожай с выбранных клеток {emotes.GetEmote(Building.Farm.ToString())} фермы.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            foreach (var userFarm in userFarms)
            {
                await _mediator.Send(new ResetUserFarmCommand(user.Id, userFarm.Number));
            }

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().Build();
            });
        }
    }
}