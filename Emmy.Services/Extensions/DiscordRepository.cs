using System.Collections.Generic;
using Emmy.Data.Enums.Discord;
using Emmy.Services.Discord.Emote.Models;
using Emmy.Services.Discord.Guild.Models;

namespace Emmy.Services.Extensions
{
    public static class DiscordRepository
    {
        public static readonly Dictionary<Channel, ChannelDto> Channels = new();
        public static readonly Dictionary<Role, RoleDto> Roles = new();
        public static readonly Dictionary<string, EmoteDto> Emotes = new();
    }
}