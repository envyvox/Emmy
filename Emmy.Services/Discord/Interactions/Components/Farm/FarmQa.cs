using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Building.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using MediatR;
using static Discord.Emote;

namespace Emmy.Services.Discord.Interactions.Components.Farm
{
    public class FarmQa : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public FarmQa(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [ComponentInteraction("farm-qa")]
        public async Task Execute(string[] selectedValues)
        {
            await Context.Interaction.DeferAsync(true);

            var selectedQuestion = selectedValues.First();

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.Farm)));

            var components = new ComponentBuilder();

            switch (selectedQuestion)
            {
                case "harvesting":
                {
                    embed
                        .WithAuthor("Выращивание урожая")
                        .WithDescription(
                            $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                            "для выращивания урожая необходимо выполнить несколько простых шагов:" +
                            $"\n\n{emotes.GetEmote("Arrow")} Для начала необходимо приобрести семена в {emotes.GetEmote("DiscordSlashCommand")} `/магазин-семян`." +
                            $"\n\n{emotes.GetEmote("Arrow")} Затем напиши {emotes.GetEmote("DiscordSlashCommand")} `/ферма` и нажми на кнопку **Посадить семена**." +
                            $"\nТы пройдешь несколько быстрых этапов, определяющих какие семена и на какие клетки {emotes.GetEmote(Building.Farm.ToString())} фермы ты хочешь посадить." +
                            $"\n\n{emotes.GetEmote("Arrow")} Семена необходимо поливать каждый день, для этого напиши {emotes.GetEmote("DiscordSlashCommand")} `/ферма` и нажми на кнопку **Полить семена**." +
                            $"\n\n{emotes.GetEmote("Arrow")} После того как семена созреют, ты можешь собрать урожай, написав {emotes.GetEmote("DiscordSlashCommand")} `/ферма` и нажав на кнопку **Собрать урожай**." +
                            $"\n\n{emotes.GetEmote("Arrow")} Если ты передумал выращивать семена или хочешь их заменить - напиши {emotes.GetEmote("DiscordSlashCommand")} `/ферма` и нажми на кнопку **Выкопать семена**." +
                            $"\nТебе необходимо будет выбрать клетки {emotes.GetEmote(Building.Farm.ToString())} фермы, с которых семена или урожай будет удален.");
                    break;
                }

                case "upgrading":
                {
                    var hasFarmExpansionL1 = await _mediator.Send(new CheckUserHasBuildingQuery(
                        user.Id, Building.FarmExpansionL1));
                    var hasFarmExpansionL2 = await _mediator.Send(new CheckUserHasBuildingQuery(
                        user.Id, Building.FarmExpansionL2));
                    var expansionL1Price = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.FarmExpansionL1Price));
                    var expansionL2Price = await _mediator.Send(new GetWorldPropertyValueQuery(
                        WorldProperty.FarmExpansionL2Price));

                    embed
                        .WithAuthor("Расширение фермы")
                        .WithDescription(
                            $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                            $"ты можешь улучшить свою {emotes.GetEmote(Building.Farm.ToString())} ферму и увеличить количество ячеек для выращивания урожая." +
                            $"\n\n{emotes.GetEmote("Arrow")} Для улучшения {emotes.GetEmote(Building.Farm.ToString())} фермы нажми на кнопку **Приобрести улучшение фермы**." +
                            $"\n\n{emotes.GetEmote(Building.FarmExpansionL1.ToString())} Первое улучшение обойдется тебе в {emotes.GetEmote(Currency.Token.ToString())} {expansionL1Price} {_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), expansionL1Price)} и откроет **2 дополнительных ячейки**." +
                            $"\n\n{emotes.GetEmote(Building.FarmExpansionL2.ToString())} Второе улучшение обойдется тебе в {emotes.GetEmote(Currency.Token.ToString())} {expansionL2Price} {_local.Localize(LocalizationCategory.Currency, Currency.Token.ToString(), expansionL2Price)} и откроет **3 дополнительных ячейки**.");

                    components.WithButton(
                        "Приобрести улучшение фермы",
                        @$"farm-upgrade:{(hasFarmExpansionL1
                            ? Building.FarmExpansionL2.GetHashCode()
                            : Building.FarmExpansionL1.GetHashCode())}",
                        emote: Parse(emotes.GetEmote(hasFarmExpansionL1
                            ? Building.FarmExpansionL2.ToString()
                            : Building.FarmExpansionL1.ToString())),
                        disabled: hasFarmExpansionL2);

                    break;
                }
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components.Build(),
                Ephemeral: true));
        }
    }
}