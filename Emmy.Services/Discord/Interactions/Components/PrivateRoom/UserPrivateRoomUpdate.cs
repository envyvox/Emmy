using System;
using System.Linq;
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
    public class UserPrivateRoomUpdate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public UserPrivateRoomUpdate(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("user-private-room-update")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync();

            var channelId = long.Parse(selectedValues.First());

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userKey = await _mediator.Send(new GetUserKeyQuery(user.Id, KeyType.PrivateRoom));

            if (userKey.Amount < 1)
            {
                throw new GameUserExpectedException(
                    $"у тебя нет в наличии {emotes.GetEmote(KeyType.PrivateRoom.EmoteName())} " +
                    $"{_local.Localize(LocalizationCategory.Key, KeyType.PrivateRoom.ToString(), 2)} " +
                    "для продления приватного сектора." +
                    $"\n\n{emotes.GetEmote("Arrow")} Приобрести его можно в {emotes.GetEmote("DiscordSlashCommand")} `/магазин-ключей`.");
            }

            await _mediator.Send(new RemoveKeyFromUserCommand(user.Id, KeyType.PrivateRoom));
            await _mediator.Send(new UpdatePrivateRoomCommand(user.Id, channelId, TimeSpan.FromDays(30)));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Приватный сектор")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно продлил приватный сектор {channelId.ToMention(MentionType.Channel)} на 30 дней. " +
                    $"Твой {emotes.GetEmote(KeyType.PrivateRoom.EmoteName())} {_local.Localize(LocalizationCategory.Key, KeyType.PrivateRoom.ToString())} был изъят.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.PrivateRoom)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}