using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Data;
using Emmy.Data.Entities.User;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Banner.Commands;
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.Container.Commands;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Referral.Queries;
using Emmy.Services.Game.Title.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Game.Referral.Commands
{
    public record CreateUserReferrerCommand(long UserId, long ReferrerId) : IRequest;

    public class CreateUserReferrerHandler : IRequestHandler<CreateUserReferrerCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;
        private readonly ILogger<CreateUserReferrerHandler> _logger;
        private readonly AppDbContext _db;

        public CreateUserReferrerHandler(
            DbContextOptions options,
            IMediator mediator,
            ILocalizationService local,
            ILogger<CreateUserReferrerHandler> logger)
        {
            _mediator = mediator;
            _local = local;
            _logger = logger;
            _db = new AppDbContext(options);
        }

        public async Task<Unit> Handle(CreateUserReferrerCommand request, CancellationToken ct)
        {
            var exist = await EntityFrameworkQueryableExtensions.AnyAsync(_db.UserReferrers,
                x => x.UserId == request.UserId);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already have referrer");
            }

            var created = await _db.CreateEntity(new UserReferrer
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ReferrerId = request.ReferrerId,
                CreatedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation(
                "Created user referrer entity {@Entity}",
                created);

            await _mediator.Send(new AddContainerToUserCommand(request.UserId, Data.Enums.Container.Token, 1));
            await AddRewardsToReferrer(request.UserId, request.ReferrerId);

            return Unit.Value;
        }

        private async Task AddRewardsToReferrer(long userId, long referrerId)
        {
            var emotes = DiscordRepository.Emotes;
            var referralCount = await _mediator.Send(new GetUserReferralCountQuery(referrerId));

            var rewardString = string.Empty;
            switch (referralCount)
            {
                case 1 or 2:

                    await _mediator.Send(new AddContainerToUserCommand(referrerId, Data.Enums.Container.Token, 1));

                    rewardString =
                        $"{emotes.GetEmote(Data.Enums.Container.Token.EmoteName())} {_local.Localize(LocalizationCategory.Container, Data.Enums.Container.Token.ToString())}";

                    break;

                case 3 or 4:

                    await _mediator.Send(new AddContainerToUserCommand(referrerId, Data.Enums.Container.Token, 2));

                    rewardString =
                        $"{emotes.GetEmote(Data.Enums.Container.Token.EmoteName())} 2 " +
                        $"{_local.Localize(LocalizationCategory.Container, Data.Enums.Container.Token.ToString(), 2)}";

                    break;

                case 5:

                    var banners = await _mediator.Send(new GetBannersQuery());
                    var banner = banners.Single(x => x.Name == "Биба и Боба");

                    await _mediator.Send(new AddContainerToUserCommand(referrerId, Data.Enums.Container.Token, 5));
                    await _mediator.Send(new AddBannerToUserCommand(referrerId, banner.Id));

                    rewardString =
                        $"{emotes.GetEmote(Data.Enums.Container.Token.EmoteName())} 5 " +
                        $"{_local.Localize(LocalizationCategory.Container, Data.Enums.Container.Token.ToString(), 5)} и " +
                        $"{emotes.GetEmote(banner.Rarity.EmoteName())} {banner.Rarity.Localize().ToLower()} баннер «{banner.Name}»";

                    break;

                case 6 or 7 or 8 or 9:

                    await _mediator.Send(new AddCurrencyToUserCommand(referrerId, Data.Enums.Currency.Lobbs, 10));

                    rewardString =
                        $"{emotes.GetEmote(Data.Enums.Currency.Lobbs.ToString())} 10 " +
                        $"{_local.Localize(LocalizationCategory.Currency, Data.Enums.Currency.Lobbs.ToString(), 10)}";

                    break;

                case 10:

                    await _mediator.Send(new AddCurrencyToUserCommand(referrerId, Data.Enums.Currency.Lobbs, 10));
                    await _mediator.Send(new AddTitleToUserCommand(referrerId, Data.Enums.Title.Yatagarasu));

                    rewardString =
                        $"{emotes.GetEmote(Data.Enums.Currency.Lobbs.ToString())} 10 " +
                        $"{_local.Localize(LocalizationCategory.Currency, Data.Enums.Currency.Lobbs.ToString(), 10)} и " +
                        $"титул {emotes.GetEmote(Data.Enums.Title.Yatagarasu.EmoteName())} {Data.Enums.Title.Yatagarasu.Localize()}";

                    break;

                case > 10:

                    await _mediator.Send(new AddCurrencyToUserCommand(referrerId, Data.Enums.Currency.Lobbs, 15));

                    rewardString =
                        $"{emotes.GetEmote(Data.Enums.Currency.Lobbs.ToString())} 15 " +
                        $"{_local.Localize(LocalizationCategory.Currency, Data.Enums.Currency.Lobbs.ToString(), 15)}";

                    break;
            }

            var user = await _mediator.Send(new GetUserQuery(userId));
            var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) user.Id));

            var embed = new EmbedBuilder()
                .WithAuthor("Реферальная система", socketUser?.GetAvatarUrl())
                .WithDescription(
                    $"{socketUser?.Mention.AsGameMention(user.Title)} указал тебя своим реферером и ты получаешь {rewardString}." +
                    $"\n\n{emotes.GetEmote("Arrow")} Напиши {emotes.GetEmote("DiscordSlashCommand")} `/приглашения` чтобы посмотреть информацию о своем участии в реферальной системе.");

            await _mediator.Send(new SendEmbedToUserCommand((ulong) referrerId, embed));
        }
    }
}