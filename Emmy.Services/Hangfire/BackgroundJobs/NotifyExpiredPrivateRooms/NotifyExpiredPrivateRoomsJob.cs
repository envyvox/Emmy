using System;
using System.Threading.Tasks;
using Discord;
using Emmy.Data;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Image = Emmy.Data.Enums.Image;

namespace Emmy.Services.Hangfire.BackgroundJobs.NotifyExpiredPrivateRooms
{
    public class NotifyExpiredPrivateRoomsJob : INotifyExpiredPrivateRoomsJob
    {
        private readonly ILogger<NotifyExpiredPrivateRoomsJob> _logger;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public NotifyExpiredPrivateRoomsJob(
            DbContextOptions options,
            ILogger<NotifyExpiredPrivateRoomsJob> logger,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Execute()
        {
            _logger.LogInformation(
                "Notify expired private rooms job executed");

            var emotes = DiscordRepository.Emotes;
            var entities = await _db.PrivateRooms
                .ToListAsync();

            entities = entities.FindAll(x => x.Expiration.Day - 3 == DateTimeOffset.UtcNow.Day);

            foreach (var entity in entities)
            {
                var user = await _mediator.Send(new GetUserQuery(entity.UserId));
                var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) user.Id));

                var embed = new EmbedBuilder()
                    .WithUserColor(user.CommandColor)
                    .WithAuthor("Приватные секторы", socketUser?.GetAvatarUrl())
                    .WithDescription(
                        $"{socketUser?.Mention.AsGameMention(user.Title)}, " +
                        $"твой приватный сектор {entity.ChannelId.ToMention(MentionType.Channel)} исстекает **через 3 дня**." +
                        $"\n\n{emotes.GetEmote("Arrow")} Не забудь заглянуть в {emotes.GetEmote("DiscordSlashCommand")} `/приватные-секторы` чтобы продлить его.")
                    .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Image.PrivateRoom)));

                await _mediator.Send(new SendEmbedToUserCommand(socketUser!.Id, embed));
            }
        }
    }
}