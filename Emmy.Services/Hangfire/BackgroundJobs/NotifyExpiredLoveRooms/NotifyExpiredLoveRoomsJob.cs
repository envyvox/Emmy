using System;
using System.Threading.Tasks;
using Discord;
using Emmy.Data;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Image = Emmy.Data.Enums.Image;

namespace Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredLoveRooms
{
    public class NotifyExpiredLoveRoomsJob : INotifyExpiredLoveRoomsJob
    {
        private readonly ILogger<NotifyExpiredLoveRoomsJob> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public NotifyExpiredLoveRoomsJob(
            DbContextOptions options,
            ILogger<NotifyExpiredLoveRoomsJob> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Notify expired love rooms job executed");

            var emotes = DiscordRepository.Emotes;
            var entities = await _db.LoveRooms
                .Include(x => x.Relationship).ThenInclude(x => x.User1)
                .Include(x => x.Relationship).ThenInclude(x => x.User2)
                .ToListAsync();

            entities = entities.FindAll(x => x.Expiration.Day - 3 == DateTimeOffset.UtcNow.Day);

            foreach (var entity in entities)
            {
                var socketUser1 = await _mediator.Send(new GetSocketGuildUserQuery(
                    (ulong) entity.Relationship.User1.Id));
                var socketUser2 = await _mediator.Send(new GetSocketGuildUserQuery(
                    (ulong) entity.Relationship.User2.Id));

                var embed1 = new EmbedBuilder()
                    .WithUserColor(entity.Relationship.User1.CommandColor)
                    .WithAuthor("Любовные гнезда")
                    .WithDescription(
                        $"{socketUser1.Mention.AsGameMention(entity.Relationship.User1.Title)}, " +
                        $"твое любовное гнездо с {socketUser2.Mention.AsGameMention(entity.Relationship.User2.Title)} исстекает **через 3 дня**." +
                        $"\n\n{emotes.GetEmote("Arrow")} Не забудьте заглянуть в {emotes.GetEmote("SlashCommand")} `/отношения` чтобы продлить его.")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Image.Relationship)));

                await _mediator.Send(new SendEmbedToUserCommand(socketUser1.Id, embed1));

                var embed2 = new EmbedBuilder()
                    .WithUserColor(entity.Relationship.User1.CommandColor)
                    .WithAuthor("Любовные гнезда")
                    .WithDescription(
                        $"{socketUser2.Mention.AsGameMention(entity.Relationship.User2.Title)}, " +
                        $"твое любовное гнездо с {socketUser1.Mention.AsGameMention(entity.Relationship.User1.Title)} исстекает **через 3 дня**." +
                        $"\n\n{emotes.GetEmote("Arrow")} Не забудьте заглянуть {emotes.GetEmote("SlashCommand")} в `/отношения` чтобы продлить его.")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Image.Relationship)));

                await _mediator.Send(new SendEmbedToUserCommand(socketUser2.Id, embed2));
            }
        }
    }
}