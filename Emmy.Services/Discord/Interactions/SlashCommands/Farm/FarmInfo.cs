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
using Emmy.Services.Game.Farm.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using MediatR;
using static Discord.Emote;
using StringExtensions = Emmy.Services.Extensions.StringExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands.Farm
{
    [RequireCommandChannel]
    [RequireLocation(Location.Neutral)]
    public class FarmInfo : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public FarmInfo(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "ферма",
            "Просмотр информации о состоянии твоей фермы")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userFarms = await _mediator.Send(new GetUserFarmsQuery(user.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ферма")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            var components = new ComponentBuilder();

            if (userFarms.Any())
            {
                embed.WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"тут отображаются твои клетки {emotes.GetEmote(Building.Farm.ToString())} фермы:" +
                    $"\n{StringExtensions.EmptyChar}");

                foreach (var userFarm in userFarms)
                {
                    string fieldName;
                    string fieldDesc;

                    switch (userFarm.State)
                    {
                        case FieldState.Empty:

                            fieldName = "Клетка земли пустая";
                            fieldDesc = "Посади на нее семена чтобы начать выращивать урожай";

                            break;
                        case FieldState.Planted:
                        {
                            var growthDays = userFarm.InReGrowth
                                ? (userFarm.Seed.ReGrowthDays - userFarm.Progress)
                                .Days().Humanize(1, new CultureInfo("ru-RU"))
                                : (userFarm.Seed.GrowthDays - userFarm.Progress)
                                .Days().Humanize(1, new CultureInfo("ru-RU"));

                            fieldName =
                                $"{emotes.GetEmote(userFarm.Seed.Name)} " +
                                $"{_local.Localize(LocalizationCategory.Seed, userFarm.Seed.Name)}, " +
                                $"еще {growthDays} роста";

                            fieldDesc = "Не забудь сегодня полить";

                            break;
                        }

                        case FieldState.Watered:
                        {
                            var growthDays = userFarm.InReGrowth
                                ? (userFarm.Seed.ReGrowthDays - userFarm.Progress)
                                .Days().Humanize(1, new CultureInfo("ru-RU"))
                                : (userFarm.Seed.GrowthDays - userFarm.Progress)
                                .Days().Humanize(1, new CultureInfo("ru-RU"));

                            fieldName =
                                $"{emotes.GetEmote(userFarm.Seed.Name)} " +
                                $"{_local.Localize(LocalizationCategory.Seed, userFarm.Seed.Name)}, " +
                                $"еще {growthDays} роста";

                            fieldDesc = "Поливать сегодня уже не нужно";

                            break;
                        }

                        case FieldState.Completed:

                            fieldName =
                                $"{emotes.GetEmote(userFarm.Seed.Crop.Name)} " +
                                $"{_local.Localize(LocalizationCategory.Crop, userFarm.Seed.Crop.Name)}, можно собирать";

                            fieldDesc = userFarm.Seed.ReGrowthDays > 0
                                ? "После сбора будет давать урожай каждые " +
                                  $"{userFarm.Seed.ReGrowthDays.Days().Humanize(1, new CultureInfo("ru-RU"))}"
                                : "Не забудь посадить что-то на освободившееся место";

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    embed.AddField($"{emotes.GetEmote("List")} `#{userFarm.Number}` {fieldName}", fieldDesc);
                }

                components
                    .WithButton(
                        "Посадить семена",
                        "farm-plant:1",
                        disabled: userFarms.Any(x => x.State is FieldState.Empty) is false)
                    .WithButton(
                        "Полить семена",
                        "farm-water",
                        disabled: userFarms.Any(x => x.State is FieldState.Planted) is false)
                    .WithButton(
                        "Собрать урожай",
                        "farm-collect",
                        disabled: userFarms.Any(x => x.State is FieldState.Completed) is false)
                    .WithButton(
                        "Выкопать семена",
                        "farm-dig",
                        ButtonStyle.Danger,
                        disabled: userFarms.All(x => x.State is FieldState.Empty))
                    .WithSelectMenu(new SelectMenuBuilder()
                        .WithPlaceholder("Выбери вопрос который тебя интересует")
                        .WithCustomId("farm-qa")
                        .AddOption(
                            "Как мне выращивать урожай?",
                            "harvesting",
                            emote: Parse(emotes.GetEmote("DiscordHelp")))
                        .AddOption(
                            "Как мне расширить ферму?",
                            "upgrading",
                            emote: Parse(emotes.GetEmote("DiscordHelp"))));
            }
            else
            {
                var farmPrice = await _mediator.Send(new GetWorldPropertyValueQuery(WorldProperty.FarmPrice));

                embed.WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"сперва тебе необходимо приобрести {emotes.GetEmote(Building.Farm.ToString())} ферму за " +
                    $"{emotes.GetEmote(Currency.Token.ToString())} {farmPrice} " +
                    $"{_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), farmPrice)}." +
                    $"\n\n{emotes.GetEmote("Arrow")} Чтобы приобрести ее, нажми кнопку **Приобрести ферму** под этим сообщением.");

                components.WithButton("Приобрести ферму", "farm-buy",
                    emote: Parse(emotes.GetEmote(Building.Farm.ToString())));
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build()));
        }
    }
}