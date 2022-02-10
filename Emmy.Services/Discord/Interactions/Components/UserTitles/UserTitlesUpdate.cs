using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.UserTitles
{
    public class UserTitlesUpdate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserTitlesUpdate(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("user-titles-update")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var title = (Title) int.Parse(selectedValues.First());

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            await _mediator.Send(new UpdateUserTitleCommand(user.Id, title));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Титулы")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(title)}, " +
                    $"ты успешно обновил свой титул на {emotes.GetEmote(title.EmoteName())} {title.Localize()}.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserTitles)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}