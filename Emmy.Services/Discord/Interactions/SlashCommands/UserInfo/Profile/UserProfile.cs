using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.Relationship.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo.Profile
{
    public class UserProfile : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserProfile(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "профиль",
            "Просмотр игрового профиля")]
        public async Task UserProfileTask(
            [Summary("пользователь", "Пользователь, профиль которого ты хочешь посмотреть")]
            IUser mentionedUser = null)
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = mentionedUser is null
                ? await _mediator.Send(new GetUserQuery((long) Context.User.Id))
                : await _mediator.Send(new GetUserQuery((long) mentionedUser.Id));
            var socketUser = mentionedUser ?? Context.User;
            var banner = await _mediator.Send(new GetUserActiveBannerQuery(user.Id));

            string relationshipString;
            var hasRelationship = await _mediator.Send(new CheckUserHasRelationshipQuery(user.Id));

            if (hasRelationship)
            {
                var relationship = await _mediator.Send(new GetUserRelationshipQuery(user.Id));
                var partner = relationship.User1.Id == user.Id ? relationship.User2 : relationship.User1;
                var socketPartner = await _mediator.Send(new GetSocketGuildUserQuery((ulong) partner.Id));

                relationshipString = $"В отношениях с {socketPartner.Mention.AsGameMention(partner.Title)}";
            }
            else
            {
                relationshipString = "Не состоит в отношениях";
            }

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Профиль")
                .WithThumbnailUrl(socketUser.GetAvatarUrl())
                .WithDescription(
                    $"{socketUser.Mention.AsGameMention(user.Title)}")
                .AddField("Пол",
                    $"{emotes.GetEmote(user.Gender.EmoteName())} {user.Gender.Localize()}", true)
                .AddField("День рождения",
                    $"Не указан {emotes.GetEmote("Blank")}", true)
                .AddField("Отношения",
                    relationshipString +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Фракция",
                    $"{emotes.GetEmote(user.Fraction.EmoteName())} {user.Fraction.Localize()}")
                .AddField("Текущая локация",
                    $"{emotes.GetEmote(user.Location.ToString())} **{user.Location.Localize()}**" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Уровень",
                    $"{emotes.GetEmote("Level")} {user.Level} уровень, {emotes.GetEmote("Xp")} {user.Xp} ед. опыта" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Дата присоединения",
                    user.CreatedAt.ToString("dd MMMM yyy", new CultureInfo("ru-RU")))
                .AddField("Информация",
                    user.About ?? "Тут пока что ничего не указано, но я уверена что это отличный пользователь.")
                .WithImageUrl(banner.Url);

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}