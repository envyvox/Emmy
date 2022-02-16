using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.GenderRequest.Commands;
using Emmy.Services.Discord.GenderRequest.Queries;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.RequestGender
{
    public class RequestGenderRole : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public RequestGenderRole(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("request-gender-role")]
        public async Task RequestGenderRoleTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            if (user.Gender is not Gender.None)
            {
                throw new GameUserExpectedException(
                    $"у тебя уже подтвержден {emotes.GetEmote(user.Gender.EmoteName())} пол.");
            }

            var roles = DiscordRepository.Roles;
            var hasGenderRequest = await _mediator.Send(new CheckUserHasGenderRequestQuery(user.Id));

            if (hasGenderRequest)
            {
                throw new GameUserExpectedException(
                    $"ты уже запрашивал подтверждение {emotes.GetEmote(Gender.None.EmoteName())} пола, " +
                    "дождись пока с тобой свяжутся " +
                    $"{roles[Data.Enums.Discord.Role.Moderator].Id.ToMention(MentionType.Role)}.");
            }

            var channels = DiscordRepository.Channels;
            var channel = await _mediator.Send(new GetSocketTextChannelQuery(channels[Channel.Moderation].Id));

            await _mediator.Send(new CreateUserGenderRequestCommand(user.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Запрос подтверждения пола")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "наши курьеры доставили твою заявку на подтверждение пола. Скоро с тобой свяжется кто-то из " +
                    $"{roles[Data.Enums.Discord.Role.Moderator].Id.ToMention(MentionType.Role)} и пригласит тебя " +
                    $"в {emotes.GetEmote("DiscordVoiceChannel")} **голосовой канал**.");

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));

            var components = new ComponentBuilder
            {
                ActionRows = new List<ActionRowBuilder>
                {
                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                                "Написал(а) ему",
                                $"gender-request-mark:{user.Id}",
                                ButtonStyle.Success,
                                emote: Parse(emotes.GetEmote("Checkmark")))
                            .Build())
                        .AddComponent(new ButtonBuilder(
                                "Не отвечает",
                                $"gender-request-delete:{user.Id}",
                                ButtonStyle.Danger,
                                emote: Parse(emotes.GetEmote("Crossmark")))
                            .Build()),

                    new ActionRowBuilder()
                        .AddComponent(new ButtonBuilder(
                                "Подтвердить мужской пол",
                                $"gender-request-update:{user.Id},{Gender.Male.GetHashCode()}",
                                emote: Parse(emotes.GetEmote(Gender.Male.EmoteName())))
                            .Build())
                        .AddComponent(new ButtonBuilder(
                                "Подтвердить женский пол",
                                $"gender-request-update:{user.Id},{Gender.Female.GetHashCode()}",
                                emote: Parse(emotes.GetEmote(Gender.Female.EmoteName())))
                            .Build())
                }
            };

            await channel.SendMessageAsync(
                $"{roles[Data.Enums.Discord.Role.Moderator].Id.ToMention(MentionType.Role)}, " +
                $"{Context.User.Mention} просит подтвердить ему {emotes.GetEmote(Gender.None.EmoteName())} пол." +
                $"\nПригласите его в {emotes.GetEmote("DiscordVoiceChannel")} **голосовой канал** для быстрой беседы.",
                components: components.Build());
        }
    }
}