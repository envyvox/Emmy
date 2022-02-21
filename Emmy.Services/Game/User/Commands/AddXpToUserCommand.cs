using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Banner.Commands;
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.Calculation;
using Emmy.Services.Game.Container.Commands;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Title.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.User.Commands
{
    public record AddXpToUserCommand(long UserId, uint Amount) : IRequest;

    public class AddXpToUserHandler : IRequestHandler<AddXpToUserCommand>
    {
        private readonly ILogger<AddXpToUserHandler> _logger;
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;
        private readonly AppDbContext _db;

        public AddXpToUserHandler(
            DbContextOptions options,
            ILogger<AddXpToUserHandler> logger,
            IMediator mediator,
            ILocalizationService local)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mediator = mediator;
            _local = local;
        }

        public async Task<Unit> Handle(AddXpToUserCommand request, CancellationToken ct)
        {
            var entity = await EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(_db.Users,
                x => x.Id == request.UserId);

            entity.Xp += request.Amount;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.UpdateEntity(entity);

            _logger.LogInformation(
                "Added xp to user {UserId} amount {Amount}",
                request.UserId, request.Amount);

            await CheckUserLevelUp(entity);

            return Unit.Value;
        }

        private async Task CheckUserLevelUp(Data.Entities.User.User user)
        {
            var xpRequired = await _mediator.Send(new GetRequiredXpQuery(user.Level + 1));

            if (user.Xp > xpRequired)
            {
                user.Level++;
                user.UpdatedAt = DateTimeOffset.UtcNow;

                await _db.UpdateEntity(user);

                _logger.LogInformation(
                    "Updated user {UserId} level to {Level}",
                    user.Id, user.Level);

                await AddLevelUpReward(user);
            }
        }

        private async Task AddLevelUpReward(Data.Entities.User.User user)
        {
            var emotes = DiscordRepository.Emotes;
            var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) user.Id));

            string rewardString;
            switch (user.Level)
            {
                // titles
                case 5 or 30 or 100:
                {
                    var title = user.Level switch
                    {
                        5 => Data.Enums.Title.FirstSamurai,
                        30 => Data.Enums.Title.Newbie, // todo change title
                        100 => Data.Enums.Title.Newbie, // todo change title
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    await _mediator.Send(new AddTitleToUserCommand(user.Id, title));

                    rewardString =
                        $"титул {emotes.GetEmote(title.EmoteName())} {title.Localize()}." +
                        $"\n\n{emotes.GetEmote("Arrow")} Найти полученный титул можно в " +
                        $"{emotes.GetEmote("DiscordSlashCommand")} `/титулы`.";

                    break;
                }
                // lobbs
                case 15 or 20 or 25 or 35 or 40 or 45 or 55 or 60 or 65 or 70 or 75 or 85 or 90 or 95:
                {
                    var amount = user.Level / 10 * 5;

                    await _mediator.Send(new AddCurrencyToUserCommand(user.Id, Data.Enums.Currency.Lobbs, amount));

                    rewardString =
                        $"{emotes.GetEmote(Data.Enums.Currency.Lobbs.ToString())} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Data.Enums.Currency.Lobbs.ToString(), amount)}." +
                        $"\n\n{emotes.GetEmote("Arrow")} Найти полученные лоббсы можно в " +
                        $"{emotes.GetEmote("DiscordSlashCommand")} `/инвентарь`.";

                    break;
                }
                // banners
                case 10 or 50 or 80:
                {
                    var banners = await _mediator.Send(new GetBannersQuery());
                    var banner = banners.Single(x => x.Name == user.Level switch
                    {
                        10 => "Первый самурай",
                        50 => "", // todo add banner name
                        80 => "", // todo add banner name
                        _ => throw new ArgumentOutOfRangeException()
                    });

                    await _mediator.Send(new AddBannerToUserCommand(user.Id, banner.Id));

                    rewardString =
                        $"{emotes.GetEmote(banner.Rarity.EmoteName())} {banner.Rarity.Localize().ToLower()} " +
                        $"баннер «{banner.Name}»." +
                        $"\n\n{emotes.GetEmote("Arrow")} Найти полученный баннер можно в " +
                        $"{emotes.GetEmote("DiscordSlashCommand")} `/банненры`.";

                    break;
                }
                // containers
                default:
                {
                    var amount = user.Level / 10 + 1;

                    await _mediator.Send(new AddContainerToUserCommand(user.Id, Data.Enums.Container.Token, amount));

                    rewardString =
                        $"{emotes.GetEmote(Data.Enums.Container.Token.EmoteName())} {amount} " +
                        $"{_local.Localize(LocalizationCategory.Container, Data.Enums.Container.Token.ToString(), amount)}." +
                        $"\n\n{emotes.GetEmote("Arrow")} Найти полученные контейнеры можно в " +
                        $"{emotes.GetEmote("DiscordSlashCommand")} `/инвентарь`.";

                    break;
                }
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Повышение уровня")
                .WithDescription(
                    $"{socketUser?.Mention.AsGameMention(user.Title)}, " +
                    $"набрав достаточное количество {emotes.GetEmote("Xp")} ед. опыта, твой уровень повышается " +
                    $"до {user.Level.AsLevelEmote()} {user.Level} и в качестве награды ты получаешь {rewardString}");

            await _mediator.Send(new SendEmbedToUserCommand(socketUser!.Id, embed));
        }
    }
}