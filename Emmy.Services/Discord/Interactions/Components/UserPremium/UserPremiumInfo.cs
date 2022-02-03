using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;

namespace Emmy.Services.Discord.Interactions.Components.UserPremium
{
    public class UserPremiumInfo : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public UserPremiumInfo(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ComponentInteraction("user-premium-info:*")]
        public async Task Execute(string pageString)
        {
            await Context.Interaction.DeferAsync(true);

            var page = uint.Parse(pageString);

            var fromMainComponent = false;
            if (page == 0)
            {
                fromMainComponent = true;
                page = 1;
            }

            var emotes = DiscordRepository.Emotes;
            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var image = page switch
            {
                1 => Data.Enums.Image.PremiumInfoRole,
                2 => Data.Enums.Image.PremiumInfoWardrobe,
                3 => Data.Enums.Image.PremiumInfoCommandColor,
                _ => throw new ArgumentOutOfRangeException(nameof(page), page, null)
            };

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"нажимай на кнопки **Назад** или **Вперед** чтобы ознакомиться со всеми преимуществами статуса {emotes.GetEmote("Premium")} премиума.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(image)));

            var components = new ComponentBuilder()
                .WithButton(
                    "Назад",
                    $"user-premium-info:{page - 1}",
                    disabled: page is 1)
                .WithButton(
                    "Вперед",
                    $"user-premium-info:{page + 1}",
                    disabled: page is 3)
                .Build();

            if (fromMainComponent)
            {
                await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed, components, Ephemeral: true));
            }
            else
            {
                await Context.Interaction.ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = embed.Build();
                    x.Components = components;
                });
            }
        }
    }
}