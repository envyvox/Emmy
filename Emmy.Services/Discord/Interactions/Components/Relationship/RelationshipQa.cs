using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Emote.Models;
using Emmy.Services.Discord.Guild.Models;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Models;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.Components.Relationship
{
    public class RelationshipQa : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        private Dictionary<string, EmoteDto> _emotes;
        private Dictionary<Channel, ChannelDto> _channels;
        private UserDto _user;

        public RelationshipQa(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("relationship-qa")]
        public async Task RelationshipQaTask(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync(true);

            var selectedQuestion = selectedValues.First();

            _emotes = DiscordRepository.Emotes;
            _channels = DiscordRepository.Channels;
            _user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            switch (selectedQuestion)
            {
                case "relationship-start":
                    await RelationshipStartTask();
                    break;
                case "relationship-end":
                    await RelationshipEndTask();
                    break;
                case "relationship-love-room-create":
                    await LoveRoomCreateTask();
                    break;
                case "relationship-love-room-update":
                    await LoveRoomUpdateTask();
                    break;
            }
        }

        private async Task RelationshipStartTask()
        {
            var embed = new EmbedBuilder()
                .WithUserColor(_user.CommandColor)
                .WithAuthor("Как начать отношения")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(_user.Title)}, " +
                    "для того чтобы начать отношения необходимо выполнить несколько простых шагов:" +
                    $"\n\n{_emotes.GetEmote("List")} У тебя и у твоего партнера должен быть подтвержденный {_emotes.GetEmote(Gender.None.EmoteName())} пол." +
                    $"\n{_emotes.GetEmote("Arrow")} Отправить запрос на подтверждение пола можно в {_channels[Channel.GetRoles].Id.ToMention(MentionType.Channel)}." +
                    $"\n\n{_emotes.GetEmote("List")} Один из вас должен написать {_emotes.GetEmote("SlashCommand")} `/предложить-отношения`, а второй принять предложение нажав на кнопку в **личных сообщениях** с ботом.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, Ephemeral: true));
        }

        private async Task RelationshipEndTask()
        {
            var embed = new EmbedBuilder()
                .WithUserColor(_user.CommandColor)
                .WithAuthor("Как закончить отношения")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(_user.Title)}, " +
                    "если вдруг отношения начали тебя тяготить, нажми на кнопку **Закончить отношения** и все исчезнет, будто ничего и не было." +
                    $"\n\n{_emotes.GetEmote("Arrow")} Если у вашей пары есть **Любовное гнездо**, то оно будет автоматически удалено после окончания отношений.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Закончить отношения",
                    "relationship-end",
                    ButtonStyle.Danger)
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components, Ephemeral: true));
        }

        private async Task LoveRoomCreateTask()
        {
            var embed = new EmbedBuilder()
                .WithUserColor(_user.CommandColor)
                .WithAuthor("Создание любовного гнезда")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(_user.Title)}, " +
                    "для создания собственного любовного гнезда необходимо выполнить несколько простых шагов:" +
                    $"\n\n{_emotes.GetEmote("List")} Находиться в отношениях." +
                    $"\n\n{_emotes.GetEmote("List")} Приобрести {_emotes.GetEmote(KeyType.LoveRoom.EmoteName())} {_local.Localize(LocalizationCategory.Key, KeyType.LoveRoom.ToString())} в {_emotes.GetEmote("SlashCommand")} `/магазин-ключей`." +
                    $"\n\n{_emotes.GetEmote("List")} Нажать кнопку **Создать любовное гнездо** под этим сообщением." +
                    $"\n{_emotes.GetEmote("Arrow")} Комната на сервере будет создана автоматически на 30 дней." +
                    $"\n{_emotes.GetEmote("Arrow")} Ты и твой партнер получите все необходимые права для общения.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Создать любовное гнездо",
                    "relationship-love-room-create",
                    emote: Parse(_emotes.GetEmote(KeyType.LoveRoom.EmoteName())))
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components, Ephemeral: true));
        }

        private async Task LoveRoomUpdateTask()
        {
            var embed = new EmbedBuilder()
                .WithUserColor(_user.CommandColor)
                .WithAuthor("Продление любовного гнезда")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(_user.Title)}, " +
                    "для продления собственного любовного гнезда необходимо выполнить несколько простых шагов:" +
                    $"\n\n{_emotes.GetEmote("List")} Приобрести новый {_emotes.GetEmote(KeyType.LoveRoom.EmoteName())} {_local.Localize(LocalizationCategory.Key, KeyType.LoveRoom.ToString())} в {_emotes.GetEmote("SlashCommand")} `/магазин-ключей`." +
                    $"\n\n{_emotes.GetEmote("List")} Нажать кнопку **Продлить любовное гнездо** под этим сообщением." +
                    $"\n{_emotes.GetEmote("Arrow")} Комната автоматически будет продлена на 30 дней.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Продлить любовное гнездо",
                    "relationship-love-room-update",
                    emote: Parse(_emotes.GetEmote(KeyType.LoveRoom.EmoteName())))
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components, Ephemeral: true));
        }
    }
}