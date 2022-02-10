using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    [RequireLocation(Location.Neutral)]
    public class Fishing : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public Fishing(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "рыбачить",
            "Отправиться рыбачить в нейтральной зоне")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Рыбалка")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"**{Location.Neutral.Localize()}** полна желающих поймать крутой улов и теперь ты один из них. " +
                    "В надежде что богиня фортуны пошлет тебе улов потяжелее ты отправляешься на рыбалку, " +
                    "но даже самые опытные рыбаки не могут знать заранее насколько удачно все пройдет." +
                    "\n\nНажми на кнопку **Бросить кубики** чтобы определить успех твоей рыбалки." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Ожидаемая награда",
                    $"{emotes.GetEmote("Xp")} опыт и {emotes.GetEmote("OctopusBW")} случайная рыба")
                .AddField("Длительность",
                    "В ожидании броска кубиков")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Fishing)));

            var components = new ComponentBuilder()
                .WithButton("Бросить кубики", $"cube-drop-fishing:{user.Id}")
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components));
        }
    }
}