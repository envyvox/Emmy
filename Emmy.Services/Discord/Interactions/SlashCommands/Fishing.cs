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
using static Emmy.Services.Extensions.ExceptionExtensions;

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

            if (user.Fraction is Data.Enums.Fraction.Neutral)
            {
                throw new GameUserExpectedException(
                    "хоть это место и зовется **Нейтральной зоной**, местные не очень " +
                    $"доверяют {emotes.GetEmote(Data.Enums.Fraction.Neutral.EmoteName())} **нейтралам** и " +
                    "не хотят отдавать свою рыбацкую лодку и снаряжение, даже на время." +
                    "\n\nТебе необходимо заручиться поддержкой фракции, ведь даже простое упоминание их имен открывает множество дверей." +
                    $"\n\n{emotes.GetEmote("Arrow")} Чтобы вступить во фракцию, напиши {emotes.GetEmote("DiscordSlashCommand")} `/фракция`.");
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Рыбалка", Context.User.GetAvatarUrl())
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