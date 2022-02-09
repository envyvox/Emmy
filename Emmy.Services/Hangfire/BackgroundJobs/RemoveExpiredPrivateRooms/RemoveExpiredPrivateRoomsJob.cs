using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Emmy.Data;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredPrivateRooms
{
    public class RemoveExpiredPrivateRoomsJob : IRemoveExpiredPrivateRoomsJob
    {
        private readonly ILogger<RemoveExpiredPrivateRoomsJob> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public RemoveExpiredPrivateRoomsJob(
            DbContextOptions options,
            ILogger<RemoveExpiredPrivateRoomsJob> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Remove expired private room job executed");

            var emotes = DiscordRepository.Emotes;
            var timeNow = DateTimeOffset.UtcNow;
            var guild = await _mediator.Send(new GetSocketGuildQuery());

            var entities = await _db.PrivateRooms
                .Include(x => x.User)
                .Where(x => x.Expiration < timeNow)
                .ToListAsync();

            foreach (var entity in entities)
            {
                await _db.DeleteEntity(entity);

                _logger.LogInformation(
                    "Deleted private room entity {@Entity}",
                    entity);

                try
                {
                    await guild.GetChannel((ulong) entity.ChannelId).DeleteAsync();

                    _logger.LogInformation(
                        "Deleted private room channel {ChannelId}",
                        entity.ChannelId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Cannot delete private room channel");
                }

                var socketUser = guild.GetUser((ulong) entity.UserId);

                var notify = new EmbedBuilder()
                    .WithUserColor(entity.User.CommandColor)
                    .WithAuthor("Приватный сектор")
                    .WithDescription(
                        $"{socketUser.Mention.AsGameMention(entity.User.Title)}, " +
                        "твой приватный сектор был удален, так как ты его не продлил." +
                        $"\n\n{emotes.GetEmote("Arrow")} Ты можешь заново создать приватный сектор заглянув в {emotes.GetEmote("DiscordSlashCommand")} `/приватные-секторы`.")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.PrivateRoom)));

                await _mediator.Send(new SendEmbedToUserCommand(socketUser.Id, notify));
            }
        }
    }
}