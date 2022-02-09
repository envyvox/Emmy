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
using Image = Emmy.Data.Enums.Image;

namespace Emmy.Services.Hangfire.BackgroundJobs.RemoveExpiredLoveRooms
{
    public class RemoveExpiredLoveRoomsJob : IRemoveExpiredLoveRoomsJob
    {
        private readonly ILogger<RemoveExpiredLoveRoomsJob> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public RemoveExpiredLoveRoomsJob(
            DbContextOptions options,
            ILogger<RemoveExpiredLoveRoomsJob> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Remove expired love rooms job executed");

            var emotes = DiscordRepository.Emotes;
            var timeNow = DateTimeOffset.UtcNow;
            var guild = await _mediator.Send(new GetSocketGuildQuery());

            var entities = await _db.LoveRooms
                .Include(x => x.Relationship).ThenInclude(x => x.User1)
                .Include(x => x.Relationship).ThenInclude(x => x.User2)
                .Where(x => x.Expiration < timeNow)
                .ToListAsync();

            foreach (var entity in entities)
            {
                await _db.DeleteEntity(entity);

                _logger.LogInformation(
                    "Deleted love room entity {@Entity}",
                    entity);

                try
                {
                    await guild.GetChannel((ulong) entity.ChannelId).DeleteAsync();

                    _logger.LogInformation(
                        "Deleted love room channel {ChannelId}",
                        entity.ChannelId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Cannot delete love room channel");
                }

                var socketUser1 = guild.GetUser((ulong) entity.Relationship.User1.Id);
                var socketUser2 = guild.GetUser((ulong) entity.Relationship.User2.Id);

                var notify1 = new EmbedBuilder()
                    .WithUserColor(entity.Relationship.User1.CommandColor)
                    .WithAuthor("Любовное гнездо")
                    .WithDescription(
                        $"{socketUser1.Mention.AsGameMention(entity.Relationship.User1.Title)}, " +
                        $"твое любовное гнездо с {socketUser2.Mention.AsGameMention(entity.Relationship.User2.Title)} было удалено, так как вы его не продлили." +
                        $"\n\n{emotes.GetEmote("Arrow")} Вы можете заново создать любовное гнездо заглянув в {emotes.GetEmote("DiscordSlashCommand")} `/отношения`.")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Image.Relationship)));

                var notify2 = new EmbedBuilder()
                    .WithUserColor(entity.Relationship.User2.CommandColor)
                    .WithAuthor("Любовное гнездо")
                    .WithDescription(
                        $"{socketUser2.Mention.AsGameMention(entity.Relationship.User2.Title)}, " +
                        $"твое любовное гнездо с {socketUser1.Mention.AsGameMention(entity.Relationship.User1.Title)} было удалено, так как вы его не продлили." +
                        $"\n\n{emotes.GetEmote("Arrow")} Вы можете заново создать любовное гнездо заглянув в {emotes.GetEmote("DiscordSlashCommand")} `/отношения`.")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Image.Relationship)));

                await _mediator.Send(new SendEmbedToUserCommand(socketUser1.Id, notify1));
                await _mediator.Send(new SendEmbedToUserCommand(socketUser2.Id, notify2));
            }
        }
    }
}