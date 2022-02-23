using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Achievement.Queries;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.Title.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Game.Achievement.Commands
{
    public record AddAchievementRewardToUserCommand(long UserId, Data.Enums.Achievement Type) : IRequest;

    public class AddAchievementRewardToUserHandler : IRequestHandler<AddAchievementRewardToUserCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public AddAchievementRewardToUserHandler(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        public async Task<Unit> Handle(AddAchievementRewardToUserCommand request, CancellationToken ct)
        {
            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery(request.UserId));
            var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) user.Id));
            var achievement = await _mediator.Send(new GetAchievementQuery(request.Type));

            string rewardString;
            switch (achievement.RewardType)
            {
                case AchievementRewardType.Lobbs:
                {
                    await _mediator.Send(new AddCurrencyToUserCommand(
                        user.Id, Data.Enums.Currency.Lobbs, achievement.RewardNumber));

                    rewardString =
                        $"{emotes.GetEmote(Data.Enums.Currency.Lobbs.ToString())} {achievement.RewardNumber} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Data.Enums.Currency.Lobbs.ToString(), achievement.RewardNumber)}." +
                        $"\n\n{emotes.GetEmote("Arrow")} Найти полученные лоббсы можно в {emotes.GetEmote("DiscordSlashCommand")} `/инвентарь`.";

                    break;
                }
                case AchievementRewardType.Title:
                {
                    var title = (Data.Enums.Title) achievement.RewardNumber;

                    await _mediator.Send(new AddTitleToUserCommand(user.Id, title));

                    rewardString =
                        $"титул {emotes.GetEmote(title.EmoteName())} {title.Localize()}." +
                        $"\n\n{emotes.GetEmote("Arrow")} Найти полученный титул можно в {emotes.GetEmote("DiscordSlashCommand")} `/титулы`.";

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Достижения", socketUser?.GetAvatarUrl())
                .WithDescription(
                    $"{socketUser?.Mention.AsGameMention(user.Title)}, " +
                    $"ты выполнил достижение {emotes.GetEmote("Achievement")} **{achievement.Name}** из категории " +
                    $"**{achievement.Category.Localize()}** и в качестве награды получаешь {rewardString}");

            return await _mediator.Send(new SendEmbedToUserCommand(socketUser!.Id, embed));
        }
    }
}