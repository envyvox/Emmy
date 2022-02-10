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
using Emmy.Services.Game.Title.Queries;
using Emmy.Services.Game.User.Queries;
using MediatR;
using static Discord.Emote;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    [RequireCommandChannel]
    public class UserTitles : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserTitles(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "титулы",
            "Просматривай и управляй своими титулами")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var titles = await _mediator.Send(new GetUserTitlesQuery(user.Id));

            titles = titles
                .Where(x => x.Type != user.Title)
                .ToList();

            var maxPage = (int) Math.Ceiling(titles.Count / 10.0);

            var components = new ComponentBuilder()
                .WithButton("Назад", "user-titles-paginator:1", disabled: true)
                .WithButton("Вперед", "user-titles-paginator:2", disabled: titles.Count <= 10);

            titles = titles
                .Take(10)
                .ToList();

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Титулы")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "тут отображаются полученные тобою титулы:" +
                    $"\n\n{emotes.GetEmote("Arrow")} Для того чтобы изменить текущий титул, **выбери его** из списка под этим сообщением." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField(
                    $"Текущий титул {emotes.GetEmote("Arrow")} {emotes.GetEmote(user.Title.EmoteName())} {user.Title.Localize()}",
                    StringExtensions.EmptyChar)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserTitles)))
                .WithFooter($"Страница 1 из {maxPage}");

            var selectMenu = new SelectMenuBuilder()
                .WithPlaceholder("Выбери титул который хочешь установить")
                .WithCustomId("user-titles-update");

            var counter = 0;
            foreach (var title in titles)
            {
                counter++;

                embed.AddField(
                    $"{emotes.GetEmote(title.Type.EmoteName())} {title.Type.Localize()}",
                    $"Получен {title.CreatedAt.ToString("dd MMMM yyy", new CultureInfo("ru-RU"))}",
                    true);

                selectMenu.AddOption(
                    title.Type.Localize().ToLower(),
                    $"{title.Type.GetHashCode()}",
                    emote: Parse(emotes.GetEmote(title.Type.EmoteName())));

                if (counter == 2)
                {
                    counter = 0;
                    embed.AddEmptyField(true);
                }
            }

            if (selectMenu.Options.Any())
            {
                components.WithSelectMenu(selectMenu);
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build()));
        }
    }
}