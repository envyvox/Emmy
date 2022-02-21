using System.Threading.Tasks;
using Discord;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Statistic.Commands;
using Emmy.Services.Game.Transit.Commands;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Hangfire.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Image = Emmy.Data.Enums.Image;

namespace Emmy.Services.Hangfire.BackgroundJobs.CompleteTransit
{
    public class CompleteTransitJob : ICompleteTransitJob
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CompleteTransitJob> _logger;

        public CompleteTransitJob(
            IMediator mediator,
            ILogger<CompleteTransitJob> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute(long userId, Location destination)
        {
            _logger.LogInformation(
                "Complete transit job executed for user {UserId} and destination {Destination}",
                userId, destination.ToString());

            var user = await _mediator.Send(new GetUserQuery(userId));
            var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) user.Id));

            await _mediator.Send(new UpdateUserLocationCommand(user.Id, destination));
            await _mediator.Send(new DeleteUserMovementCommand(user.Id));
            await _mediator.Send(new DeleteUserHangfireJobCommand(user.Id, HangfireJobType.Transit));
            await _mediator.Send(new AddStatisticToUserCommand(user.Id, Statistic.Transit));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Отправления", socketUser?.GetAvatarUrl())
                .WithDescription(
                    $"{socketUser?.Mention.AsGameMention(user.Title)}, " +
                    $"ты достиг точки прибытия, добро пожаловать в **{destination.Localize()}**.")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Image.Transit)));

            await _mediator.Send(new SendEmbedToUserCommand(socketUser!.Id, embed));
        }
    }
}