using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Emote.Models;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Currency.Models;
using Emmy.Services.Game.Currency.Queries;
using Emmy.Services.Game.Key.Models;
using Emmy.Services.Game.Key.Queries;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands.UserInfo
{
    public class UserInventory : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;
        private Dictionary<string, EmoteDto> _emotes;

        public UserInventory(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "инвентарь",
            "Просмотр игрового инвентаря")]
        public async Task UserInventoryTask()
        {
            await Context.Interaction.DeferAsync(true);

            _emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCurrencies = await _mediator.Send(new GetUserCurrenciesQuery(user.Id));
            var userKeys = await _mediator.Send(new GetUserKeysQuery(user.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Инвентарь")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    "все полученные предметы попадают сюда:" +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Валюта", DisplayUserCurrencies(userCurrencies))
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.UserInventory)));

            if (userKeys.Any())
            {
                embed.AddField("Ключи", DisplayUserKeys(userKeys));
            }

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }

        private string DisplayUserCurrencies(Dictionary<Currency, UserCurrencyDto> userCurrencies)
        {
            var str = Enum
                .GetValues(typeof(Currency))
                .Cast<Currency>()
                .Aggregate(string.Empty, (s, v) =>
                    s +
                    $"{_emotes.GetEmote(v.ToString())} {(userCurrencies.ContainsKey(v) ? userCurrencies[v].Amount : 0)} " +
                    $"{_local.Localize(LocalizationCategory.Currency, v.ToString(), userCurrencies.ContainsKey(v) ? userCurrencies[v].Amount : 0)}, ");

            return str.RemoveFromEnd(2);
        }

        private string DisplayUserKeys(List<UserKeyDto> userKeys)
        {
            var str = userKeys.Aggregate(string.Empty, (s, v) =>
                s +
                $"{_emotes.GetEmote(v.Type.EmoteName())} {v.Amount} {_local.Localize(LocalizationCategory.Key, v.Type.ToString(), v.Amount)}, ");

            return str.RemoveFromEnd(2);
        }
    }
}