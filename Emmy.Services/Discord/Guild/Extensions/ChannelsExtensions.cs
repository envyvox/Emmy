using System;
using System.Collections.Generic;
using System.Linq;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Guild.Models;

namespace Emmy.Services.Discord.Guild.Extensions
{
    public static class ChannelsExtensions
    {
        public static IEnumerable<ulong> GetCommunityDescChannels(this Dictionary<Channel, ChannelDto> channels)
        {
            var communityDescChannels = Enum
                .GetValues(typeof(Channel))
                .Cast<Channel>()
                .Where(x => x.Parent() == Channel.CommunityDescParent);

            return channels
                .Where(x => communityDescChannels.Contains(x.Key))
                .Select(x => x.Value.Id);
        }
    }
}