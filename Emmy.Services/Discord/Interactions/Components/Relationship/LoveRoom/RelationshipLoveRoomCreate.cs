using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.LoveRoom.Commands;
using Emmy.Services.Discord.LoveRoom.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Key.Commands;
using Emmy.Services.Game.Key.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Relationship.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.Components.Relationship.LoveRoom
{
    public class RelationshipLoveRoomCreate : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public RelationshipLoveRoomCreate(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("relationship-love-room-create")]
        public async Task RelationshipLoveRoomCreateTask()
        {
            await Context.Interaction.DeferAsync();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var hasRelationship = await _mediator.Send(new CheckUserHasRelationshipQuery(user.Id));

            if (hasRelationship is false)
            {
                throw new GameUserExpectedException(
                    "создать любовное гнездо можно лишь находясь в отношениях.");
            }

            var relationship = await _mediator.Send(new GetUserRelationshipQuery(user.Id));
            var hasLoveRoom = await _mediator.Send(new CheckRelationshipHasLoveRoomQuery(relationship.Id));

            if (hasLoveRoom)
            {
                throw new GameUserExpectedException(
                    "у вашей пары уже есть любовное гнездо, если ты хочешь его продлить - то тебе необходимо " +
                    $"выбрать диалог {emotes.GetEmote("DiscordHelp")} «Как мне продлить любовное гнездо?».");
            }

            var userKey = await _mediator.Send(new GetUserKeyQuery(user.Id, KeyType.LoveRoom));

            if (userKey.Amount < 1)
            {
                throw new GameUserExpectedException(
                    $"у тебя нет в наличии {emotes.GetEmote(KeyType.PrivateRoom.EmoteName())} " +
                    $"{_local.Localize(LocalizationCategory.Key, KeyType.PrivateRoom.ToString(), 2)} " +
                    "для создания любовного гнезда." +
                    $"\n\n{emotes.GetEmote("Arrow")} Приобрести его можно в {emotes.GetEmote("DiscordSlashCommand")} `/магазин-ключей`.");
            }

            await _mediator.Send(new RemoveKeyFromUserCommand(user.Id, KeyType.LoveRoom));
            var channelId = await _mediator.Send(new CreateLoveRoomCommand(relationship, TimeSpan.FromDays(30)));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Любовное гнездо")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно создал любовное гнездо {channelId.ToMention(MentionType.Channel)} на 30 дней. " +
                    $"Твой {emotes.GetEmote(KeyType.LoveRoom.EmoteName())} {_local.Localize(LocalizationCategory.Key, KeyType.LoveRoom.ToString())} был изъят.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Relationship)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}