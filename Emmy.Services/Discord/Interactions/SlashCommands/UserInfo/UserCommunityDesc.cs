using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.CommunityDesc.Models;
using Emmy.Services.Discord.CommunityDesc.Queries;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Emote.Models;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Discord.Role.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using Humanizer;
using MediatR;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    [RequireCommandChannel]
    public class UserCommunityDesc : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;
        private Dictionary<string, EmoteDto> _emotes;

        public UserCommunityDesc(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "доска-сообщества",
            "Информация о твоем участии в доске сообщества")]
        public async Task UserCommunityDescTask()
        {
            await Context.Interaction.DeferAsync(true);

            _emotes = DiscordRepository.Emotes;
            var channels = DiscordRepository.Channels;
            var roles = DiscordRepository.Roles;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var userMessages = await _mediator.Send(new GetContentMessagesByUserIdQuery(user.Id));
            var userVotes = await _mediator.Send(new GetContentAuthorVotesQuery(user.Id));
            var hasRole = await _mediator.Send(new CheckGuildUserHasRoleByTypeQuery(
                Context.User.Id, Data.Enums.Discord.Role.ContentProvider));

            var photosMessages = userMessages
                .Where(x => x.ChannelId == (long) channels[Channel.Photos].Id)
                .ToList();
            var screenshotMessages = userMessages
                .Where(x => x.ChannelId == (long) channels[Channel.Screenshots].Id)
                .ToList();
            var memesMessages = userMessages
                .Where(x => x.ChannelId == (long) channels[Channel.Memes].Id)
                .ToList();
            var artMessages = userMessages
                .Where(x => x.ChannelId == (long) channels[Channel.Arts].Id)
                .ToList();
            var musicMessages = userMessages
                .Where(x => x.ChannelId == (long) channels[Channel.Music].Id)
                .ToList();
            var eroticMessages = userMessages
                .Where(x => x.ChannelId == (long) channels[Channel.Erotic].Id)
                .ToList();
            var nsfwMessages = userMessages
                .Where(x => x.ChannelId == (long) channels[Channel.Nsfw].Id)
                .ToList();

            var photosMessagesLikes = (uint) ChannelMessagesVotes(userVotes, photosMessages, Vote.Like);
            var photosMessagesDislikes = (uint) ChannelMessagesVotes(userVotes, photosMessages, Vote.Dislike);
            var screenshotMessagesLikes = (uint) ChannelMessagesVotes(userVotes, screenshotMessages, Vote.Like);
            var screenshotMessagesDislikes = (uint) ChannelMessagesVotes(userVotes, screenshotMessages, Vote.Dislike);
            var memesMessagesLikes = (uint) ChannelMessagesVotes(userVotes, memesMessages, Vote.Like);
            var memesMessagesDislikes = (uint) ChannelMessagesVotes(userVotes, memesMessages, Vote.Dislike);
            var artMessagesLikes = (uint) ChannelMessagesVotes(userVotes, artMessages, Vote.Like);
            var artMessagesDislikes = (uint) ChannelMessagesVotes(userVotes, artMessages, Vote.Dislike);
            var musicMessagesLikes = (uint) ChannelMessagesVotes(userVotes, musicMessages, Vote.Like);
            var musicMessagesDislikes = (uint) ChannelMessagesVotes(userVotes, musicMessages, Vote.Dislike);
            var eroticMessagesLikes = (uint) ChannelMessagesVotes(userVotes, eroticMessages, Vote.Like);
            var eroticMessagesDislikes = (uint) ChannelMessagesVotes(userVotes, eroticMessages, Vote.Dislike);
            var nsfwMessagesLikes = (uint) ChannelMessagesVotes(userVotes, nsfwMessages, Vote.Like);
            var nsfwMessagesDislikes = (uint) ChannelMessagesVotes(userVotes, nsfwMessages, Vote.Dislike);

            var totalLikes = (uint) userVotes.Count(x => x.Vote == Vote.Like);
            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Доска сообщества", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут собрана информация о твоем участии в доске сообщества: " +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField(Channel.Photos.Name(),
                    DisplayChannelInfo((uint) photosMessages.Count,
                        photosMessagesLikes, photosMessagesDislikes), true)
                .AddField(Channel.Screenshots.Name(),
                    DisplayChannelInfo((uint) screenshotMessages.Count,
                        screenshotMessagesLikes, screenshotMessagesDislikes), true)
                .AddField(Channel.Memes.Name(),
                    DisplayChannelInfo((uint) memesMessages.Count,
                        memesMessagesLikes, memesMessagesDislikes), true)
                .AddField(Channel.Arts.Name(),
                    DisplayChannelInfo((uint) artMessages.Count,
                        artMessagesLikes, artMessagesDislikes), true)
                .AddField(Channel.Music.Name(),
                    DisplayChannelInfo((uint) musicMessages.Count,
                        musicMessagesLikes, musicMessagesDislikes), true)
                .AddField(Channel.Erotic.Name(),
                    DisplayChannelInfo((uint) eroticMessages.Count,
                        eroticMessagesLikes, eroticMessagesDislikes), true)
                .AddField(Channel.Nsfw.Name(),
                    DisplayChannelInfo((uint) nsfwMessages.Count,
                        nsfwMessagesLikes, nsfwMessagesDislikes), true)
                .AddField("Всего",
                    $"{_emotes.GetEmote(Vote.Like.ToString())} {totalLikes} " +
                    $"{_local.Localize(LocalizationCategory.Vote, Vote.Like.ToString(), totalLikes)}");

            if (hasRole)
            {
                var userRole = await _mediator.Send(new GetUserRoleQuery(
                    user.Id, (long) roles[Data.Enums.Discord.Role.ContentProvider].Id));
                var roleEndString = (DateTimeOffset.UtcNow - (DateTimeOffset) userRole.Expiration!).TotalDays
                    .Days()
                    .Humanize(2, new CultureInfo("ru-RU"));

                embed.AddField("Поставщик контента",
                    $"Роль будет снята через {roleEndString}");
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }

        private static int ChannelMessagesVotes(List<ContentVoteDto> votes, List<ContentMessageDto> messages, Vote vote)
        {
            return votes
                .Where(cv => messages
                    .Any(cm => cv.ContentMessage.Id == cm.Id))
                .Count(cv => cv.Vote == vote);
        }

        private string DisplayChannelInfo(uint messages, uint likes, uint dislikes)
        {
            return
                $"{_emotes.GetEmote("List")} {messages} {_local.Localize(LocalizationCategory.Basic, "Post", messages)}" +
                $"\n{_emotes.GetEmote(Vote.Like.ToString())} {likes} {_local.Localize(LocalizationCategory.Vote, Vote.Like.ToString(), likes)}" +
                $"\n{_emotes.GetEmote(Vote.Dislike.ToString())} {dislikes} {_local.Localize(LocalizationCategory.Vote, Vote.Dislike.ToString(), dislikes)}";
        }
    }
}