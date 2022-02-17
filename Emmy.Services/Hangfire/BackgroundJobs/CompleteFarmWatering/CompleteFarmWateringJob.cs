using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Farm.Commands;
using Emmy.Services.Game.Transit.Commands;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Hangfire.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Image = Emmy.Data.Enums.Image;

namespace Emmy.Services.Hangfire.BackgroundJobs.CompleteFarmWatering
{
    public class CompleteFarmWateringJob : ICompleteFarmWateringJob
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CompleteFarmWateringJob> _logger;

        public CompleteFarmWateringJob(
            IMediator mediator,
            ILogger<CompleteFarmWateringJob> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute(long userId)
        {
            _logger.LogInformation(
                "Complete farm watering job executed for user {UserId}",
                userId);

            var user = await _mediator.Send(new GetUserQuery(userId));
            var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) user.Id));

            await _mediator.Send(new UpdateUserLocationCommand(user.Id, Location.Neutral));
            await _mediator.Send(new DeleteUserMovementCommand(user.Id));
            await _mediator.Send(new UpdateUserFarmsStateCommand(user.Id, FieldState.Watered));
            await _mediator.Send(new DeleteUserHangfireJobCommand(user.Id, HangfireJobType.FarmWatering));

            var embed = new EmbedBuilder()
                .WithAuthor("Ферма")
                .WithDescription(
                    $"{socketUser?.Mention.AsGameMention(user.Title)}, " +
                    "ты успешно полил семена, теперь можно быть уверенным в том, что они будут расти.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Image.Farm)));

            await _mediator.Send(new SendEmbedToUserCommand(socketUser!.Id, embed));
        }
    }
}