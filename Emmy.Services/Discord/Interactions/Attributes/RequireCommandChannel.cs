using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Extensions;

namespace Emmy.Services.Discord.Interactions.Attributes
{
    public class RequireCommandChannel : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
            ICommandInfo commandInfo, IServiceProvider services)
        {
            var emotes = DiscordRepository.Emotes;
            var channels = DiscordRepository.Channels;

            return await Task.FromResult(
                context.Channel.Id == channels[Channel.Commands].Id
                    ? PreconditionResult.FromSuccess()
                    : PreconditionResult.FromError(
                        $"использовать {emotes.GetEmote("DiscordSlashCommand")} команды необходимо " +
                        $"в канале {channels[Channel.Commands].Id.ToMention(MentionType.Channel)}."));
        }
    }
}