using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Emmy.Data.Enums;
using Emmy.Services.Discord.Embed;
using Emmy.Services.Discord.Guild.Commands;
using Emmy.Services.Discord.Interactions.Attributes;
using Emmy.Services.Extensions;
using Emmy.Services.Game.Cooldown.Commands;
using Emmy.Services.Game.Cooldown.Queries;
using Emmy.Services.Game.User.Queries;
using Emmy.Services.Game.World.Queries;
using Humanizer;
using MediatR;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Interactions.SlashCommands
{
    [RequireCommandChannel]
    public class Rename : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public Rename(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand(
            "изменить-никнейм",
            "Сменить серверное имя")]
        public async Task Execute(
            [Summary("никнейм", "Новое серверное имя")] [MinValue(1)] [MaxValue(32)]
            string username)
        {
            await Context.Interaction.DeferAsync();

            var user = await _mediator.Send(new GetUserQuery((long) Context.User.Id));
            var userCooldown = await _mediator.Send(new GetUserCooldownQuery(user.Id, Cooldown.Rename));

            if (userCooldown.Expiration > DateTimeOffset.UtcNow)
            {
                throw new GameUserExpectedException(
                    "ты недавно уже изменял серверное имя, не стоит менять его так часто, " +
                    $"попробуй еще раз **{userCooldown.Expiration.Humanize(culture: new CultureInfo("ru-RU"))}**.");
            }

            var durationInHours = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.RenameCooldownDurationInHours));

            await _mediator.Send(new AddCooldownToUserCommand(
                user.Id, Cooldown.Rename, TimeSpan.FromHours(durationInHours)));
            await _mediator.Send(new RenameGuildUserCommand(Context.User.Id, username));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Изменение никнейма", Context.User.GetAvatarUrl())
                .WithDescription(
                    $"{Context.User.Mention.AsGameMention(user.Title)}, " +
                    $"ты успешно изменил свое серверное имя на **{username}**.");

            await _mediator.Send(new FollowUpEmbedCommand(Context.Interaction, embed));
        }
    }
}