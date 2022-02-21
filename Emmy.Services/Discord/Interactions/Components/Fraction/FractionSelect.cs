using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Fraction.Queries;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Fraction
{
    public class FractionSelect : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public FractionSelect(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("fraction-select")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var fraction = (Data.Enums.Fraction) int.Parse(selectedValues.First());

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            if (user.Fraction is not Data.Enums.Fraction.Neutral)
            {
                throw new GameUserExpectedException(
                    $"ты уже состоишь во фракции {emotes.GetEmote(user.Fraction.EmoteName())} **{user.Fraction.Localize()}**.");
            }

            var redRoseCount = await _mediator.Send(new GetFractionUsersCountQuery(
                Data.Enums.Fraction.RedRose));
            var whiteCrowCount = await _mediator.Send(new GetFractionUsersCountQuery(
                Data.Enums.Fraction.WhiteCrow));
            var goldenSharkCount = await _mediator.Send(new GetFractionUsersCountQuery(
                Data.Enums.Fraction.GoldenShark));
            var allowableCountAdvantage = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.FractionAllowableCountAdvantage));

            if (fraction is Data.Enums.Fraction.RedRose &&
                redRoseCount > whiteCrowCount + allowableCountAdvantage &&
                redRoseCount > goldenSharkCount + allowableCountAdvantage ||
                fraction is Data.Enums.Fraction.WhiteCrow &&
                whiteCrowCount > redRoseCount + allowableCountAdvantage &&
                whiteCrowCount > goldenSharkCount + allowableCountAdvantage ||
                fraction is Data.Enums.Fraction.GoldenShark &&
                goldenSharkCount > redRoseCount + allowableCountAdvantage &&
                goldenSharkCount > whiteCrowCount + allowableCountAdvantage)
            {
                throw new GameUserExpectedException(
                    $"во фракции {emotes.GetEmote(fraction.EmoteName())} **{fraction.Localize()}** " +
                    "сейчас слишком много участников. Ты можешь попробовать вступить позже или выбрать другую фракцию.");
            }

            await _mediator.Send(new UpdateUserFractionCommand(user.Id, fraction));
            await _mediator.Send(new RemoveRoleFromGuildUserByRoleTypeCommand(
                Context.User.Id, Data.Enums.Discord.Role.FractionNeutral));
            await _mediator.Send(new AddRoleToGuildUserByRoleTypeCommand(Context.User.Id, fraction.Role()));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Фракция", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно вступил во фракцию {emotes.GetEmote(fraction.EmoteName())} **{fraction.Localize()}**." +
                    $"\n\n{emotes.GetEmote("Arrow")} Загляни теперь в {emotes.GetEmote("DiscordSlashCommand")} `/фракция` чтобы посмотреть информацию о текущем состоянии твоей фракции.");

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}