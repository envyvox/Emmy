using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Localization;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    public class DonateInfo : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _local;

        public DonateInfo(
            IMediator mediator,
            ILocalizationService local)
        {
            _mediator = mediator;
            _local = local;
        }

        [SlashCommand(
            "поддержка",
            "Узнай как ты можешь поддержать развитие сервера")]
        public async Task Execute()
        {
            await Context.Interaction.DeferAsync(true);

            var emotes = DiscordRepository.Emotes;
            var roles = DiscordRepository.Roles;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Поддержка сервера")
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты можешь поддержать развие сервера копеечкой и получить {emotes.GetEmote(Currency.Lobbs.ToString())} лоббсы в благодарность." +
                    $"\n\n{emotes.GetEmote("Arrow")} За каждый пожертвованный {emotes.GetEmote("Ruble")} рубль ты получишь {emotes.GetEmote(Currency.Lobbs.ToString())} {_local.Localize(LocalizationCategory.Currency, Currency.Lobbs.ToString())}." +
                    $"\n{StringExtensions.EmptyChar}")
                .AddField("Реквизиты",
                    $"{emotes.GetEmote("Monobank")} `5375 4141 0460 6651` GARBUZOV EUGENE" +
                    $"\n{emotes.GetEmote("Sberbank")} `4276 6800 1181 7390` CHERNYKH TATIANA" +
                    $"\n\n{emotes.GetEmote("Arrow")} После перевода необходимо сделать скриншот оплаты и отправить в личные сообщения {roles[Data.Enums.Discord.Role.Administration].Id.ToMention(MentionType.Role)}.")
                .WithFooter("Ты можешь обратиться в личные сообщения по любым вопросам")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.DonateInfo)));

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}