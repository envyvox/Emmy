using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.PrivateRoom.Commands;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Key.Commands;
using Emmy.Services.Game.Key.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.PrivateRoom
{
    public class UserPrivateRoomCreate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public UserPrivateRoomCreate(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("user-private-room-create")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userKey = await _mediator.Send(new GetUserKeyQuery(user.Id, KeyType.PrivateRoom));

            if (userKey.Amount < 1)
            {
                throw new GameUserExpectedException(
                    $"у тебя нет в наличии {emotes.GetEmote(KeyType.PrivateRoom.EmoteName())} " +
                    $"{_local.Localize(LocalizationCategory.Key, KeyType.PrivateRoom.ToString(), 2)} " +
                    "для создания приватного сектора." +
                    $"\n\n{emotes.GetEmote("Arrow")} Приобрести его можно в {emotes.GetEmote("SlashCommand")} `/магазин-ключей`.");
            }

            await _mediator.Send(new RemoveKeyFromUserCommand(user.Id, KeyType.PrivateRoom));
            var channelId = await _mediator.Send(new CreatePrivateRoomCommand(user.Id, TimeSpan.FromDays(30)));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Приватные секторы")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно создал приватный сектор {channelId.ToMention(MentionType.Channel)} на 30 дней. " +
                    $"Твой {emotes.GetEmote(KeyType.PrivateRoom.EmoteName())} {_local.Localize(LocalizationCategory.Key, KeyType.PrivateRoom.ToString())} был изъят.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.PrivateRoom)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}