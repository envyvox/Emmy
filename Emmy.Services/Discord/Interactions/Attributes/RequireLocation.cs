using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Emote.Extensions;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Emmy.Services.Discord.Interactions.Attributes
{
    public class RequireLocation : PreconditionAttribute
    {
        private readonly Location _requiredLocation;

        public RequireLocation(Location requiredLocation)
        {
            _requiredLocation = requiredLocation;
        }

        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
            ICommandInfo commandInfo, IServiceProvider services)
        {
            var emotes = DiscordRepository.Emotes;
            var service = services.GetRequiredService<IMediator>();
            var user = await service.Send(new GetUserQuery((long) context.User.Id));

            return user.Location == _requiredLocation
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError(
                    $"это действие доступно лишь в **{_requiredLocation.Localize(true)}**, напиши " +
                    $"{emotes.GetEmote("SlashCommand")} `/отправления` и выбери соответствующую локацию.");
        }
    }
}