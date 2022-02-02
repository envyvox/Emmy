using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Enums.Discord;
using Emmy.Data.Extensions;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Discord.Role.Commands;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.Donate.Commands;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Discord.Emote;
using static Emmy.Data.Enums.Discord.Role;

namespace Emmy.Services.Discord.Interactions.SlashCommands.Administration
{
    [Attributes.RequireRole(Data.Enums.Discord.Role.Administration)]
    [Group("adm", "Admin commands")]
    public class AdministrationCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public AdministrationCommands(
            IMediator mediator,
            DbContextOptions options)
        {
            _db = new AppDbContext(options);
            _mediator = mediator;
        }

        [SlashCommand("add-currency", "Add currency to user")]
        public async Task AdmAddCurrencyToUserTask(IUser user, Currency currency, uint amount)
        {
            await Context.Interaction.DeferAsync(true);
            await _mediator.Send(new AddCurrencyToUserCommand((long) user.Id, currency, amount));
            await Context.Interaction.FollowupAsync("Ok");
        }

        [SlashCommand("add-donate", "Add donate to user")]
        public async Task AdmAddDonateToUserTask(IUser user, uint amount)
        {
            await Context.Interaction.DeferAsync(true);
            await _mediator.Send(new AddDonateToUserCommand((long) user.Id, amount));
            await _mediator.Send(new AddCurrencyToUserCommand((long) user.Id, Currency.Lobbs, amount));
            await Context.Interaction.FollowupAsync("Ok");
        }

        [SlashCommand("add-role", "Add a role to user")]
        public async Task AdmAddRoleToUserTask(IUser user, IRole role, uint days, bool isPersonal = false,
            bool autoToggle = false)
        {
            await Context.Interaction.DeferAsync(true);

            await _mediator.Send(new AddRoleToUserCommand(
                (long) user.Id, (long) role.Id, TimeSpan.FromDays(days), isPersonal));

            if (autoToggle)
            {
                await _mediator.Send(new AddRoleToGuildUserByRoleIdCommand(user.Id, role.Id));
            }

            await Context.Interaction.FollowupAsync("Ok");
        }

        [SlashCommand("update-gender", "Update user gender")]
        public async Task AdmUpdateUserGenderTask(IUser user, Gender gender)
        {
            await Context.Interaction.DeferAsync(true);
            await _mediator.Send(new UpdateUserGenderCommand((long) user.Id, gender));
            await Context.Interaction.FollowupAsync("Ok");
        }

        [SlashCommand("preset-welcome", "Sends a preset message with welcome")]
        public async Task AdmPresetWelcomeTask()
        {
            await Context.Interaction.DeferAsync(true);

            var channels = DiscordRepository.Channels;
            var roles = DiscordRepository.Roles;
            var emotes = DiscordRepository.Emotes;

            await Context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                .WithDefaultColor()
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Welcome)))
                .Build());

            await Context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                .WithDefaultColor()
                .WithTitle("Навигатор по серверу")
                .WithDescription(
                    $"{channels[Channel.Chat].Id.ToMention(MentionType.Channel)} - Общение на различные темы.\n" +
                    $"{channels[Channel.Commands].Id.ToMention(MentionType.Channel)} - Взаимодействие с ботом.\n" +
                    $"{channels[Channel.GetRoles].Id.ToMention(MentionType.Channel)} - Получение серверных ролей.\n" +
                    $"{channels[Channel.Announcements].Id.ToMention(MentionType.Channel)} - Серверные оповещения.\n" +
                    "\n**Игровая вселенная**\n" +
                    $"{channels[Channel.GameInfo].Id.ToMention(MentionType.Channel)} - Информация о боте.\n" +
                    $"{channels[Channel.GameLore].Id.ToMention(MentionType.Channel)} - История игрового мира.\n" +
                    $"{channels[Channel.GameEvents].Id.ToMention(MentionType.Channel)} - Оповещения об игровых событиях.\n" +
                    $"{channels[Channel.GameUpdates].Id.ToMention(MentionType.Channel)} - Обновления бота.\n" +
                    "\n**Доска сообщества**\n" +
                    $"{channels[Channel.CommunityDescHowItWork].Id.ToMention(MentionType.Channel)} - Описание того, как устроена доска.\n" +
                    $"{channels[Channel.Photos].Id.ToMention(MentionType.Channel)} - Красивые ~~котики~~ фотографии.\n" +
                    $"{channels[Channel.Screenshots].Id.ToMention(MentionType.Channel)} - Твои яркие моменты.\n" +
                    $"{channels[Channel.Memes].Id.ToMention(MentionType.Channel)} - Говорящее само за себя название канала.\n" +
                    $"{channels[Channel.Arts].Id.ToMention(MentionType.Channel)} - Красивые рисунки.\n" +
                    $"{channels[Channel.Music].Id.ToMention(MentionType.Channel)} - Любимые треки или клипы.\n" +
                    $"{channels[Channel.Erotic].Id.ToMention(MentionType.Channel)} - Изображения, носящие эротический характер.\n" +
                    $"{channels[Channel.Nsfw].Id.ToMention(MentionType.Channel)} - Изображения 18+.\n" +
                    "\n**Великая «Тосёкан»**\n" +
                    $"{channels[Channel.Rules].Id.ToMention(MentionType.Channel)} - Правила сервера.\n" +
                    $"{channels[Channel.Giveaways].Id.ToMention(MentionType.Channel)} - Различные серверные розыгрыши.\n" +
                    $"{channels[Channel.Suggestions].Id.ToMention(MentionType.Channel)} - Твои предложения по улучшению сервера или бота.\n" +
                    "\n**Голосовые каналы**\n" +
                    "На нашем сервере присутствует возможность **создавать** голосовые каналы самостоятельно, " +
                    $"для этого необходимо **зайти** в канал {channels[Channel.CreateRoom].Id.ToMention(MentionType.Channel)}.\n" +
                    $"{emotes.GetEmote("Arrow")} Ты можешь изменять название и все права созданной тобой комнаты.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.WelcomeVoiceChannels)))
                .Build());

            await Context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                .WithDefaultColor()
                .WithTitle("Мероприятия")
                .WithDescription(
                    "На нашем сервере проводятся различные мероприятия, такие как кастомные игры 5х5 в " +
                    "League of Legends, Valorant или сломанный телефон, мафия и другие.\n\n" +
                    "**Расписание мероприятий**\n" +
                    "Для того чтобы всегда быть в курсе всех мероприятий, нажми на кнопку " +
                    $"{emotes.GetEmote("DiscordScheduledEvent")} **Меропрития** над текстовыми каналами, " +
                    "там отображаются все **запланированные** мероприятия.\n\n" +
                    "Выбери интересующее тебя мероприятие и нажми\n" +
                    $"{emotes.GetEmote("DiscordNotification")} **Интересует** для того чтобы получить уведомление о его " +
                    $"начале, а так же чтобы {roles[EventManager].Id.ToMention(MentionType.Role)} было проще " +
                    "понять сколько людей собирается придти.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.WelcomeScheduledEvents)))
                .Build());

            await Context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                .WithDefaultColor()
                .WithTitle("Игровая вселенная")
                .WithDescription(
                    $"На сервере присутствует собственный RPG бот {Context.Client.CurrentUser.Mention}.\n\n" +
                    "Развитие персонажа, экономика, выращивание урожая, рыбалка, изготовление и многое другое " +
                    "ждет тебя в твоем приключении по игровому миру.\n\n" +
                    $"Заглядывай в {channels[Channel.GameInfo].Id.ToMention(MentionType.Channel)} чтобы узнать подробнее.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.WelcomeGameWorld)))
                .Build());

            await Context.Interaction.FollowupAsync("Ok");
        }

        [SlashCommand("preset-rules", "Sends a preset message with rules")]
        public async Task AdmPresetRulesTask()
        {
            await Context.Interaction.DeferAsync(true);

            // todo rules messages

            await Context.Interaction.FollowupAsync("Ok");
        }

        [SlashCommand("preset-game-roles", "Send a preset message with game roles")]
        public async Task AdmPresetGameRolesTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var gameRoles = RoleHelper.GameRoles();

            var embed = new EmbedBuilder()
                .WithDefaultColor()
                .WithAuthor("Игровые роли")
                .WithDescription(
                    "Ты можешь получить **игровые роли**, которые открывают доступ к **текстовым каналам** где можно " +
                    "как **найти людей** для совместной игры, так и просто пообщаться на игровую тематику. " +
                    "Для этого **выбери роли** из списка под этим сообщением.")
                .WithFooter("Игровые роли можно при необходимости снять, убрав их из списка.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.GetGameRoles)));

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери игровые роли")
                .WithCustomId("select-game-roles")
                .WithMinValues(0)
                .WithMaxValues(gameRoles.Count);

            foreach (var gameRole in gameRoles)
            {
                selectMenu.AddOption(
                    gameRole.Name(),
                    $"{gameRole.GetHashCode()}",
                    emote: Parse(emotes.GetEmote(gameRole.ToString())));
            }

            await Context.Channel.SendMessageAsync(embed: embed.Build(),
                components: new ComponentBuilder()
                    .WithSelectMenu(selectMenu)
                    .Build());

            await Context.Interaction.FollowupAsync("Ok");
        }

        [SlashCommand("preset-gender-request", "Send a preset message with gender request")]
        public async Task AdmPresetGenderRequestTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var roles = DiscordRepository.Roles;

            var embed = new EmbedBuilder()
                .WithDefaultColor()
                .WithAuthor("Подтверждение пола")
                .WithDescription(
                    "Ты можешь запросить **подтверждение пола** и получить роль " +
                    $"<@&{roles[GenderMale].Id}> или <@&{roles[GenderFemale].Id}>, " +
                    "открывающую доступ к особому **голосовому каналу**, доступному только пользователям с этим полом, а так же отображение пола " +
                    $"{emotes.GetEmote(Gender.Male.EmoteName())}{emotes.GetEmote(Gender.Female.EmoteName())} " +
                    "в **игровом профиле**. Для этого **нажми на кнопку** под этим сообщением." +
                    $"\n\n{emotes.GetEmote("Arrow")} После нажатия на кнопку, с тобой свяжутся " +
                    $"{roles[Moderator].Id.ToMention(MentionType.Role)} и пригласят тебя в " +
                    $"{emotes.GetEmote("DiscordVoiceChannel")} **голосовой канал** для подтверждения пола.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.RequestGenderRole)))
                .WithFooter("Гендерную роль можно при необходимости снять, попросив об этом администратора.");

            var button = new ComponentBuilder()
                .WithButton("Запросить роль", "request-gender-role");

            await Context.Channel.SendMessageAsync(embed: embed.Build(), components: button.Build());
            await Context.Interaction.FollowupAsync("Ok");
        }

        [SlashCommand("preset-how-desc-work", "Sends a preset message with how desc work")]
        public async Task AdmPresetHowDescWorkTask()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var channels = DiscordRepository.Channels;
            var roles = DiscordRepository.Roles;

            var embed = new EmbedBuilder()
                .WithDefaultColor()
                .WithAuthor("Доска сообщества")
                .WithDescription(
                    "Ты можешь делиться с нами своими любимыми изображениями в каналах доски сообщества." +
                    $"\n\n{emotes.GetEmote("Arrow")} Напиши в канал {channels[Channel.Commands].Id.ToMention(MentionType.Channel)} " +
                    "`/доска-сообщества` чтобы посмотреть информацию о своем участии." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Каналы доски",
                    $"{emotes.GetEmote("List")} {channels[Channel.Photos].Id.ToMention(MentionType.Channel)} - Красивые ~~котики~~ фотографии.\n" +
                    $"{emotes.GetEmote("List")} {channels[Channel.Screenshots].Id.ToMention(MentionType.Channel)} - Твои яркие моменты.\n" +
                    $"{emotes.GetEmote("List")} {channels[Channel.Memes].Id.ToMention(MentionType.Channel)} - Говорящее само за себя название канала.\n" +
                    $"{emotes.GetEmote("List")} {channels[Channel.Arts].Id.ToMention(MentionType.Channel)} - Красивые рисунки.\n" +
                    $"{emotes.GetEmote("List")} {channels[Channel.Music].Id.ToMention(MentionType.Channel)} - Твоя любимая музыка.\n" +
                    $"{emotes.GetEmote("List")} {channels[Channel.Erotic].Id.ToMention(MentionType.Channel)} - Изображения, носящие эротический характер.\n" +
                    $"{emotes.GetEmote("List")} {channels[Channel.Nsfw].Id.ToMention(MentionType.Channel)} - Изображения 18+." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Получение роли",
                    $"Пользователи, публикации который набирают суммарно {emotes.GetEmote("Like")} 500 лайков " +
                    $"получают роль {roles[ContentProvider].Id.ToMention(MentionType.Role)} на 30 дней." +
                    $"\n\n{emotes.GetEmote("Arrow")} Если пользователь получит еще {emotes.GetEmote("Like")} 500 лайков " +
                    "уже имея роль, то ее длительность увеличится на 30 дней." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Модерация",
                    $"Публикации набирающие {emotes.GetEmote("Dislike")} 5 дизлайков будут автоматически удалены." +
                    $"\n\n{emotes.GetEmote("Arrow")} Публикации нарушающие правила сервера или правила отдельных " +
                    "каналов будут удалены модерацией сервера без предупреждения.");

            await Context.Channel.SendMessageAsync("", false, embed.Build());
            await Context.Interaction.FollowupAsync("Ok");
        }

        [SlashCommand("preset-game-info", "Sends a preset message with game info")]
        public async Task AdmPresetGameInfoTask()
        {
            await Context.Interaction.DeferAsync(true);

            // todo game info messages

            await Context.Interaction.FollowupAsync("Ok");
        }
    }
}