using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.GenderRequest.Commands;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.RequestGender
{
    public class GenderRequestUpdate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public GenderRequestUpdate(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("gender-request-update:*,*")]
        public async Task GenderRequestUpdateTask(string userIdString, string genderHashcode)
        {
            await Context.Interaction.DeferAsync(true);

            var userId = long.Parse(userIdString);
            var gender = (Gender) int.Parse(genderHashcode);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery(userId));
            var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) userId));

            await _mediator.Send(new UpdateUserGenderRequestCommand(
                user.Id, RequestState.Verified, (long) Context.User.Id));
            await _mediator.Send(new UpdateUserGenderCommand(user.Id, gender));
            await _mediator.Send(new AddRoleToGuildUserByRoleTypeCommand((ulong) user.Id, gender.Role()));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Запрос подтверждения пола", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{socketUser?.Mention.AsGameMention(user.Title)}, " +
                    $"твой пол обновлен на {emotes.GetEmote(gender.EmoteName())} {gender.Localize().ToLower()}.");

            await _mediator.Send(new SendEmbedToUserCommand((ulong) userId, embed));

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Content =
                    $"Модератор {Context.User.Mention} подтвердил пользователю {socketUser?.Mention} " +
                    $"{emotes.GetEmote(gender.EmoteName())} {gender.Localize().ToLower()} пол.";
                x.Components = new ComponentBuilder().Build();
            });
        }
    }
}