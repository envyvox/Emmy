using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Achievement.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    [RequireCommandChannel]
    public class UserAchievements : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public UserAchievements(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "достижения",
            "Просматривай всевозможные достижения и награды за их выполнение")]
        public async Task Execute(
            [Summary("категория", "Категория достижений которую ты хочешь посмотреть")]
            [Choice("Первые шаги", 1)]
            [Choice("Рыбалка", 2)]
            [Choice("Выращивание урожая", 3)]
            [Choice("Казино", 4)]
            [Choice("Торговля", 5)]
            [Choice("Фракция", 6)]
            [Choice("Коллекция", 7)]
            int categoryHashcode)
        {
            await Context.Interaction.DeferAsync(true);

            var category = (AchievementCategory) categoryHashcode;

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var achievements = await _mediator.Send(new GetAchievementsQuery(category));
            var userAchievements = await _mediator.Send(new GetUserAchievementsQuery(user.Id, category));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Достижения", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"тут отображаются твои достижения в категории **{category.Localize()}**:" +
                    $"\n{StringExtensions.EmptyChar}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserAchievements)));

            foreach (var achievement in achievements)
            {
                var userAchievement = userAchievements.SingleOrDefault(x => x.Achievement.Type == achievement.Type);
                var exist = userAchievement is not null;

                embed.AddField($"{emotes.GetEmote("Achievement" + (exist ? "" : "BW"))} {achievement.Name}",
                    "Награда: " +
                    achievement.RewardType switch
                    {
                        AchievementRewardType.Lobbs =>
                            $"{emotes.GetEmote(Currency.Lobbs.ToString())} {achievement.RewardNumber} " +
                            $"{_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString(), achievement.RewardNumber)}",
                        AchievementRewardType.Title =>
                            $"титул {emotes.GetEmote(((Title) achievement.RewardNumber).EmoteName())} " +
                            $"{((Title) achievement.RewardNumber).Localize()}",
                        _ => throw new ArgumentOutOfRangeException()
                    } + (exist
                        ? $"\nВыполнено в {userAchievement.CreatedAt.ToString("HH:MM, dd MMMM yyyy", new CultureInfo("ru-RU"))}"
                        : ""));
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}