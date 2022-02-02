using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.GenderRequest.Commands;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.RequestGender
{
    public class GenderRequestDelete : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public GenderRequestDelete(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("gender-request-delete:*")]
        public async Task GenderRequestDeleteTask(string userIdString)
        {
            await Context.Interaction.DeferAsync(true);

            var userId = long.Parse(userIdString);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery(userId));
            var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) userId));

            await _mediator.Send(new DeleteUserGenderRequestCommand(user.Id));

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Content =
                    $"Модератор {Context.User.Mention} отметил что пользователь {socketUser.Mention} " +
                    $"не отвечает на приглашение в {emotes.GetEmote("DiscordVoiceChannel")} **голосовой канал**." +
                    $"\nЗапрос на подтверждение {emotes.GetEmote(Gender.None.EmoteName())} пола был удален.";
                x.Components = new ComponentBuilder().Build();
            });
        }
    }
}