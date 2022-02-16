using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Cooldown.Queries;
using Emmy.Services.Game.Fraction.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using MediatR;
using static Discord.Emote;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    public class Fraction : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public Fraction(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "фракция",
            "Информация о фракциях")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            if (user.Fraction is Data.Enums.Fraction.Neutral)
            {
                var embed = new EmbedBuilder()
                    .WithUserColor(user.CommandColor)
                    .WithAuthor("Фракция")
                    .WithDescription(
                        $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                        "тебе необходимо выбрать в какую фракцию ты хочешь вступить, " +
                        "для этого **выбери ее** из списка под этим сообщением.");

                var selectMenu = new SelectMenuBuilder()
                    .WithPlaceholder("Выбери фракцию в которую ты хочешь вступить")
                    .WithCustomId("fraction-select");

                foreach (var fraction in Enum
                    .GetValues(typeof(Emmy.Data.Enums.Fraction))
                    .Cast<Data.Enums.Fraction>()
                    .Where(x => x is not Data.Enums.Fraction.Neutral))
                {
                    embed.AddField(
                        StringExtensions.EmptyChar,
                        $"Фракция {emotes.GetEmote(fraction.EmoteName())} **{fraction.Localize()}** " +
                        $"расположеная в **{fraction.Location().Localize(true)}**" +
                        $"\n\n{emotes.GetEmote("Arrow")} {fraction.Description()}");

                    selectMenu.AddOption(
                        fraction.Localize(),
                        $"{fraction.GetHashCode()}",
                        emote: Parse(emotes.GetEmote(fraction.EmoteName())));
                }

                await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed,
                    new ComponentBuilder().WithSelectMenu(selectMenu).Build()));
            }
            else
            {
                var fraction = await _mediator.Send(new GetFractionQuery(user.Fraction));
                var fractionGiftPrice = await _mediator.Send(new GetWorldPropertyValueQuery(
                    WorldProperty.FractionGiftPrice));
                var fractionRaidCooldownInHours = await _mediator.Send(new GetWorldPropertyValueQuery(
                    WorldProperty.FractionRaidCooldownDurationInHours));
                var fractionGiftCooldownInHours = await _mediator.Send(new GetWorldPropertyValueQuery(
                    WorldProperty.FractionGiftCooldownDurationInHours));

                var userRaidCooldown = await _mediator.Send(new GetUserCooldownQuery(user.Id, Cooldown.FractionRaid));
                var userGiftCooldown = await _mediator.Send(new GetUserCooldownQuery(user.Id, Cooldown.FractionGift));

                var embed = new EmbedBuilder()
                    .WithUserColor(user.CommandColor)
                    .WithAuthor("Фракция")
                    .WithDescription(
                        $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                        $"ты состоишь во фракции {emotes.GetEmote(user.Fraction.EmoteName())} **{user.Fraction.Localize()}**." +
                        $"\n{StringExtensions.EmptyChar}")
                    .AddField(
                        $"{emotes.GetEmote("Arrow")} {emotes.GetEmote("FractionPoints")} {fraction.Points} " +
                        $"{_local.Localize(LocalizationCategory.Basic, "FractionPoints", fraction.Points)}",
                        StringExtensions.EmptyChar)
                    .AddField("Еженедельные задания",
                        "Для того чтобы фракция чувствовала себя уверенной, необходимо активно принимать участие " +
                        "во всем что происходит вокруг. Мы можем направить тебя в твоем приключении, которое принесет пользу и тебе и нам." +
                        $"\n\n{emotes.GetEmote("Arrow")} Для получения заданий нажми на кнопку **Получить еженедельные задания**." +
                        $"\n{StringExtensions.EmptyChar}")
                    .AddField("Ежедневные задания",
                        "Для того чтобы фракция чувствовала себя уверенной, необходимо активно принимать участие " +
                        "во всем что происходит вокруг. Мы можем направить тебя в твоем приключении, которое принесет пользу и тебе и нам." +
                        $"\n\n{emotes.GetEmote("Arrow")} Для получения заданий нажми на кнопку **Получить ежедненые задания**." +
                        $"\n{StringExtensions.EmptyChar}")
                    .AddField("Вылазки",
                        "Мы постоянно отправляемся на территорию других фракций в поисках чего-то, что можно " +
                        "быстро и незаметно унести с собой. Ты можешь присоединиться к нам и мы поделим с тобой " +
                        $"часть {emotes.GetEmote(Container.Token.EmoteName())}{emotes.GetEmote(Container.Supply.EmoteName())} добычи." +
                        $"\nНо не чаще чем раз в **{fractionRaidCooldownInHours.Hours().Humanize(culture: new CultureInfo("ru-RU"))}**, " +
                        "иначе нас можно будет легко подловить." +
                        $"\n\n{emotes.GetEmote("Arrow")} Чтобы начать вылазку нажми на кнопку **Начать вылазку**." +
                        $"\n{StringExtensions.EmptyChar}")
                    .AddField("Отправка благотворительных припасов",
                        $"Всего за каких-то {emotes.GetEmote(Currency.Token.ToString())} {fractionGiftPrice} " +
                        $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), fractionGiftPrice)} " +
                        $"ты можешь отправить {emotes.GetEmote(Container.Token.EmoteName())}{emotes.GetEmote(Container.Supply.EmoteName())} " +
                        "припасы случайному пользователю своей фракции." +
                        $"\nНо не чаще чем раз в **{fractionGiftCooldownInHours.Hours().Humanize(culture: new CultureInfo("ru-RU"))}**, " +
                        "у нас не так много свободных курьеров." +
                        $"\n\n{emotes.GetEmote("Arrow")} Для отправки припасов нажми на кнопку **Отправить припасы**.");

                var components = new ComponentBuilder()
                    .WithButton(
                        "Получить еженедельные задания",
                        "fraction-quests-weekly",
                        emote: Parse(emotes.GetEmote("FractionQuestWeekly")),
                        disabled: true // todo включить когда будет работать функционал
                    )
                    .WithButton(
                        "Получить ежедневные задания",
                        "fraction-quests-daily",
                        emote: Parse(emotes.GetEmote("FractionQuestDaily")),
                        disabled: true // todo включить когда будет работать функционал
                    )
                    .WithButton(
                        "Начать вылазку",
                        "fraction-raid",
                        emote: Parse(emotes.GetEmote("FractionRaid")),
                        disabled: true
                        // todo включить когда будет работать функционал
                        // userRaidCooldown.Expiration > DateTimeOffset.UtcNow
                    )
                    .WithButton(
                        "Отправить припасы",
                        "fraction-gift",
                        emote: Parse(emotes.GetEmote(Container.Token.EmoteName())),
                        disabled: userGiftCooldown.Expiration > DateTimeOffset.UtcNow)
                    .Build();

                await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components));
            }
        }
    }
}