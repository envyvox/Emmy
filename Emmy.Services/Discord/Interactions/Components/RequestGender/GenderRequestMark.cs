using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.Components.RequestGender
{
    public class GenderRequestMark : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public GenderRequestMark(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("gender-request-mark:*")]
        public async Task GenderRequestMarkTask(string userIdString)
        {
            await Context.Interaction.DeferAsync(true);

            var userId = ulong.Parse(userIdString);
            var emotes = DiscordRepository.Emotes;
            var socketUser = await _mediator.Send(new GetSocketGuildUserQuery(userId));

            var components = new ComponentBuilder
            {
                ActionRows = new List<ActionRowBuilder>
                {
                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                            "Не отвечает",
                            $"gender-request-delete:{userId}",
                            ButtonStyle.Danger,
                            emote: Parse(emotes.GetEmote("Crossmark"))).Build()),

                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                            "Подтвердить мужской пол",
                            $"gender-request-update:{userId},{Gender.Male.GetHashCode()}",
                            emote: Parse(emotes.GetEmote(Gender.Male.EmoteName()))).Build())
                        .AddComponent(new ButtonBuilder(
                            "Подтвердить женский пол",
                            $"gender-request-update:{userId},{Gender.Female.GetHashCode()}",
                            emote: Parse(emotes.GetEmote(Gender.Female.EmoteName()))).Build())
                }
            };

            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Content =
                    $"Модератор {Context.User.Mention} уже написал в личные сообщения пользователю {socketUser.Mention} " +
                    $"и пригласил его в {emotes.GetEmote("DiscordVoiceChannel")} **голосовой канал** " +
                    $"для подтверждения {emotes.GetEmote(Gender.None.EmoteName())} пола.";
                x.Components = components.Build();
            });
        }
    }
}