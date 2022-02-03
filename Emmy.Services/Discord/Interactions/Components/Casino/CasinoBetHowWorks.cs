using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.Casino
{
    public class CasinoBetHowWorks : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public CasinoBetHowWorks(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("casino-bet-how-works")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Как работают ставки")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "после того как ты делаешь ставку - бросаются кубики которые и определяют твой успех:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Если на кубиках выпадает **от 1 до 54** включительно, ты **проигрываешь и теряешь** поставленные {emotes.GetEmote(Currency.Token.ToString())} токены" +
                    $"\n\n{emotes.GetEmote("Arrow")} Если на кубиках выпадает **от 55 до 89** включительно, ты **побеждаешь и получаешь х2** от поставленных тобой {emotes.GetEmote(Currency.Token.ToString())} токенов" +
                    $"\n\n{emotes.GetEmote("Arrow")} Если на кубиках выпадает **от 90 до 99** включительно, ты **побеждаешь и получаешь х4** от поставленных тобой {emotes.GetEmote(Currency.Token.ToString())} токенов" +
                    $"\n\n{emotes.GetEmote("Arrow")} Если на кубиках выпадает **100**, ты **побеждаешь и получаешь х10** от поставленных тобой {emotes.GetEmote(Currency.Token.ToString())} токенов")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Casino)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));
        }
    }
}