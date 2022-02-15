using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Fraction.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using Humanizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    [Group("рейтинг", "Просматривай серверные и игровые рейтинги")]
    public class WorldRating : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;
        private readonly AppDbContext _db;

        public WorldRating(
            DbContextOptions options,
            IMediator mediator,
            ILocalizationService local)
        {
            _db = new AppDbContext(options);
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand("токенов", "Рейтинг пользователей по токенам")]
        public async Task RatingCurrencyTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var entities = await _db.UserCurrencies
                .AmountNotZero()
                .Include(x => x.User)
                .Where(x =>
                    x.User.OnGuild &&
                    x.Type == Currency.Token)
                .OrderByDescending(x => x.Amount)
                .Take(10)
                .ToListAsync();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Рейтинг по токенам")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Rating)));

            if (entities.Any())
            {
                for (var pos = 1; pos < entities.Count; pos++)
                {
                    var current = entities[pos - 1];
                    var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) current.User.Id));

                    if (socketUser is null) continue;

                    var currentUser = await _mediator.Send(new GetUserQuery(current.User.Id));

                    embed.AddField(StringExtensions.EmptyChar,
                        $"{pos.AsPositionEmote()} `{pos}` {socketUser.Mention.AsGameMention(currentUser.Title)} " +
                        $"{emotes.GetEmote("Arrow")} {emotes.GetEmote(Currency.Token.ToString())} {current.Amount} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), current.Amount)}");
                }
            }
            else
            {
                embed.AddField(StringExtensions.EmptyChar,
                    "В этом рейтинге еще никого нет, самое время тебе стать первым!");
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }

        [SlashCommand("сообщений", "Рейтинг пользователей по сообщениям за неделю")]
        public async Task RatingMessagesTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var entities = await _db.UserStatistics
                .Include(x => x.User)
                .Where(x =>
                    x.User.OnGuild &&
                    x.Type == Statistic.Messages)
                .OrderByDescending(x => x.Amount)
                .Take(10)
                .ToListAsync();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Рейтинг по сообщениям")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Rating)));

            if (entities.Any())
            {
                for (var i = 1; i <= entities.Count; i++)
                {
                    var current = entities[i - 1];
                    var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) current.User.Id));

                    if (socketUser is null) continue;

                    var currentUser = await _mediator.Send(new GetUserQuery(current.User.Id));

                    embed.AddField(StringExtensions.EmptyChar,
                        $"{i.AsPositionEmote()} `{i}` {socketUser.Mention.AsGameMention(currentUser.Title)} " +
                        $"{emotes.GetEmote("Arrow")} {current.Amount} " +
                        $"{_local.Localize(LocalizationCategory.Basic, "Message", current.Amount)}");
                }
            }
            else
            {
                embed.AddField(StringExtensions.EmptyChar,
                    "В этом рейтинге еще никого нет, самое время тебе стать первым!");
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }

        [SlashCommand("голосовой", "Рейтинг пользователей по проведенному времени в голосовых каналах за неделю")]
        public async Task RatingMinutesInVoiceTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var entities = await _db.UserStatistics
                .Include(x => x.User)
                .Where(x =>
                    x.User.OnGuild &&
                    x.Type == Statistic.MinutesInVoice)
                .OrderByDescending(x => x.Amount)
                .Take(10)
                .ToListAsync();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Рейтинг по голосовой активности")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Rating)));

            if (entities.Any())
            {
                for (var i = 1; i <= entities.Count; i++)
                {
                    var current = entities[i - 1];
                    var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) current.User.Id));

                    if (socketUser is null) continue;

                    var currentUser = await _mediator.Send(new GetUserQuery(current.User.Id));

                    embed.AddField(StringExtensions.EmptyChar,
                        $"{i.AsPositionEmote()} `{i}` {socketUser.Mention.AsGameMention(currentUser.Title)} " +
                        $"{emotes.GetEmote("Arrow")} {current.Amount.Minutes().Humanize(2, new CultureInfo("ru-RU"))}");
                }
            }
            else
            {
                embed.AddField(StringExtensions.EmptyChar,
                    "В этом рейтинге еще никого нет, самое время тебе стать первым!");
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }

        [SlashCommand("пожертвований", "Рейтинг пользователей по количеству пожертвованных рублей")]
        public async Task RatingDonateTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var entities = await _db.UserDonations
                .Include(x => x.User)
                .Where(x => x.User.OnGuild)
                .OrderByDescending(x => x.Amount)
                .Take(10)
                .ToListAsync();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Рейтинг по пожертвованиям")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Rating)));

            if (entities.Any())
            {
                for (var i = 1; i <= entities.Count; i++)
                {
                    var current = entities[i - 1];
                    var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) current.User.Id));

                    if (socketUser is null) continue;

                    var currentUser = await _mediator.Send(new GetUserQuery(current.User.Id));

                    embed.AddField(StringExtensions.EmptyChar,
                        $"{i.AsPositionEmote()} `{i}` {socketUser.Mention.AsGameMention(currentUser.Title)} " +
                        $"{emotes.GetEmote("Arrow")} {emotes.GetEmote("Ruble")} {current.Amount} " +
                        $"{_local.Localize(LocalizationCategory.Basic, "Ruble", current.Amount)}");
                }
            }
            else
            {
                embed.AddField(StringExtensions.EmptyChar,
                    "В этом рейтинге еще никого нет, самое время тебе стать первым!");
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }

        [SlashCommand("опыта", "Рейтинг пользователей по количеству полученного опыта")]
        public async Task RatingExpTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var entities = await _db.Users
                .AsQueryable()
                .OrderByDescending(x => x.Xp)
                .ToListAsync();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Рейтинг по опыту")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Rating)));

            for (var i = 1; i <= entities.Count; i++)
            {
                var current = entities[i - 1];
                var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) current.Id));

                if (socketUser is null) continue;

                var currentUser = await _mediator.Send(new GetUserQuery(current.Id));

                embed.AddField(StringExtensions.EmptyChar,
                    $"{i.AsPositionEmote()} `{i}` {socketUser.Mention.AsGameMention(currentUser.Title)} " +
                    $"{emotes.GetEmote("Arrow")} {current.Level.AsLevelEmote()} {current.Level} уровень, {emotes.GetEmote("Xp")} {current.Xp} ед. опыта");
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }

        [SlashCommand("фракций", "Рейтинг фракций по количество накопленных очков")]
        public async Task RatingFractionTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var fractions = await _db.Fractions
                .AsQueryable()
                .OrderByDescending(x => x.Points)
                .ToListAsync();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Рейтинг фракций")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Rating)));

            for (var i = 1; i <= fractions.Count; i++)
            {
                var current = fractions[i - 1];

                embed.AddField(StringExtensions.EmptyChar,
                    $"{i.AsPositionEmote()} `{i}` {emotes.GetEmote(current.Type.EmoteName())} " +
                    $"**{current.Type.Localize()}** {emotes.GetEmote("Arrow")} {emotes.GetEmote("FractionPoints")} " +
                    $"{current.Points} {_local.Localize(LocalizationCategory.Basic, "FractionPoints", current.Points)}");
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}