using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Emote.Models;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.PrivateRoom.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Models;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.Components.PrivateRoom
{
    public class FamilyRoomQa : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;
        private Dictionary<string, EmoteDto> _emotes;
        private UserDto _user;

        public FamilyRoomQa(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("private-room-qa")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync(true);

            var selectedQuestion = selectedValues.First();

            _emotes = DiscordRepository.Emotes;
            _user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            switch (selectedQuestion)
            {
                case "private-room-create":
                    await Create();
                    break;
                case "private-room-update":
                    await Update();
                    break;
            }
        }

        private async Task Create()
        {
            var embed = new EmbedBuilder()
                .WithUserColor(_user.CommandColor)
                .WithAuthor("Создание и настройка приватного сектора")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(_user.Title)}, " +
                    "для создания собственного приватного сектора необходимо выполнить несколько простых шагов:" +
                    $"\n\n{_emotes.GetEmote("List")} Приобрести {_emotes.GetEmote(KeyType.PrivateRoom.EmoteName())} {_local.Localize(LocalizationCategory.Key, KeyType.PrivateRoom.ToString())} в {_emotes.GetEmote("SlashCommand")} `/магазин-ключей`." +
                    $"\n\n{_emotes.GetEmote("List")} Нажать кнопку **Создать приватный сектор** под этим сообщением." +
                    $"\n{_emotes.GetEmote("Arrow")} Комната на сервере будет создана автоматически и записана на твое имя на 30 дней." +
                    $"\n{_emotes.GetEmote("Arrow")} Ты получишь все необходимые права для управления созданной комнатой." +
                    $"\n\n{_emotes.GetEmote("List")} На созданном канале нажать {_emotes.GetEmote("DiscordSettings")} **Настроить канал** и переименовать его под свой вкус." +
                    $"\n\n{_emotes.GetEmote("List")} Перейти во вкладку **Права доступа** и добавить своих друзей, выдав им права **Просмотр канала** и **Подключаться**." +
                    $"\n{_emotes.GetEmote("Arrow")} Все остальные права, необходимые для общения, будут доступны по-умолчанию.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.PrivateRoom)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Создать приватный сектор",
                    "user-private-room-create",
                    emote: Parse(_emotes.GetEmote(KeyType.PrivateRoom.EmoteName())))
                .Build();

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components, Ephemeral: true));
        }

        private async Task Update()
        {
            var privateRooms = await _mediator.Send(new GetUserPrivateRoomsQuery(_user.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(_user.CommandColor)
                .WithAuthor("Продление приватного сектора")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(_user.Title)}, " +
                    "для продения собственного приватного сектора еще на 30 дней необходимо выполнить несколько простых шагов:" +
                    $"\n\n{_emotes.GetEmote("List")} Приобрести новый {_emotes.GetEmote(KeyType.PrivateRoom.EmoteName())} {_local.Localize(LocalizationCategory.Key, KeyType.PrivateRoom.ToString())} в {_emotes.GetEmote("SlashCommand")} `/магазин-ключей`." +
                    $"\n\n{_emotes.GetEmote("List")} Выбрать необходимый сектор из списка под этим сообщением." +
                    $"\n{_emotes.GetEmote("Arrow")} Комната автоматически будет продлена на 30 дней.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.PrivateRoom)));

            var components = new ComponentBuilder();
            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери сектор который хочешь продлить")
                .WithCustomId("user-private-room-update");

            if (privateRooms.Any())
            {
                var guild = await _mediator.Send(new GetSocketGuildQuery());

                foreach (var familyRoom in privateRooms)
                {
                    var channel = guild.GetVoiceChannel((ulong) familyRoom.ChannelId);

                    selectMenu.AddOption(
                        channel.Name.ToLower(),
                        $"{channel.Id}",
                        emote: Parse(_emotes.GetEmote(KeyType.PrivateRoom.EmoteName())));
                }
            }

            if (selectMenu.Options.Any())
            {
                components.WithSelectMenu(selectMenu);
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build(),
                Ephemeral: true));
        }
    }
}