using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Emmy.Services.Discord.Interactions.Attributes
{
    public class RequirePremium : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
            ICommandInfo commandInfo, IServiceProvider services)
        {
            var emotes = DiscordRepository.Emotes;
            var service = services.GetRequiredService<IMediator>();
            var user = await service.Send(new GetUserQuery((long) context.User.Id));

            return user.IsPremium
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError(
                    $"это действие доступно лишь {emotes.GetEmote("Premium")} премиум пользователям.");
        }
    }
}