using Emmy.Data.Enums.Discord;

namespace Emmy.Services.Discord.Guild.Models
{
    public record ChannelDto(
        ulong Id,
        Channel Type);
}