using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Emote.Models;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Models;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.Wardrobe
{
    [RequirePremium]
    public class UserWardrobeQa : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        private Dictionary<string, EmoteDto> _emotes;
        private UserDto _user;

        public UserWardrobeQa(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("user-wardrobe-qa")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync(true);

            var selectedQuestion = selectedValues.First();

            _emotes = DiscordRepository.Emotes;
            _user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            switch (selectedQuestion)
            {
                case "user-wardrobe-create":
                    await Create();
                    break;
                case "user-wardrobe-update":
                    await Update();
                    break;
            }
        }

        private async Task Create()
        {
            var embed = new EmbedBuilder()
                .WithUserColor(_user.CommandColor)
                .WithAuthor("Гардеробная")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(_user.Title)}, " +
                    $"для создания собственной роли напиши {_emotes.GetEmote("SlashCommand")} `/гардеробная-создать` и укажи желаемые название и цвет роли." +
                    $"\n\n{_emotes.GetEmote("Arrow")} Указать цвет необходимо в HEX формате, получить который можно например [в google color picker](https://www.google.com/search?q=colorpicker).")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Wardrobe)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));
        }

        private async Task Update()
        {
            var embed = new EmbedBuilder()
                .WithUserColor(_user.CommandColor)
                .WithAuthor("Гардеробная")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(_user.Title)}, " +
                    $"для обновления собственной роли напиши {_emotes.GetEmote("SlashCommand")} `/гардеробная-обновить` и укажи новые название и цвет роли." +
                    $"\n\n{_emotes.GetEmote("Arrow")} Указать цвет необходимо в HEX формате, получить который можно например [в google color picker](https://www.google.com/search?q=colorpicker).")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Wardrobe)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));
        }
    }
}