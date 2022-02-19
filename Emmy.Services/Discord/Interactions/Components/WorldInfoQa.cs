using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components
{
    public class WorldInfoQa : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public WorldInfoQa(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("world-info-qa")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync(true);

            var selectedQuestion = selectedValues.First();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.WorldInfo)));

            switch (selectedQuestion)
            {
                case "timesDay":

                    embed
                        .WithAuthor("Время суток")
                        .WithDescription(
                            $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                            "время суток влияет на виды рыб, которые ты можешь поймать во время рыбалки. " +
                            $"Некоторую рыбу можно поймать лишь {emotes.GetEmote("Night")} ночью или наоборот " +
                            $"только {emotes.GetEmote("Day")} днем.");

                    break;
                case "weather":

                    embed
                        .WithAuthor("Погода")
                        .WithDescription(
                            $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                            "погода влияет на виды рыб, которые ты можешь поймать во время рыбалки. " +
                            $"Некоторую рыбу можно поймать лишь в {emotes.GetEmote("WeatherRain")} дождь или наоборот " +
                            $"только при {emotes.GetEmote("WeatherClear")} солнечной погоде.");

                    break;
                case "season":

                    embed
                        .WithAuthor("Сезон")
                        .WithDescription(
                            $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                            "текущий сезон определяет ассортимент семян в магазине, а так же виды рыб, которые ты можешь " +
                            "поймать во время рыбалки. Некоторую рыбу можно поймать лишь в определенный сезон.");

                    break;
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));
        }
    }
}