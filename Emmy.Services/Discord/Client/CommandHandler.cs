using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Emmy.Services.Discord.Image.Queries;
using Emmy.Services.Extensions;
using Emmy.Services.Game.User.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using static Emmy.Services.Extensions.ExceptionExtensions;

namespace Emmy.Services.Discord.Client
{
    public class CommandHandler
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IMediator _mediator;

        private DiscordSocketClient _client;
        private InteractionService _commands;
        private IServiceProvider _services;

        public CommandHandler(
            ILogger<CommandHandler> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task InitializeAsync(DiscordSocketClient discordSocketClient,
            InteractionService interactionService, IServiceProvider serviceProvider)
        {
            _client = discordSocketClient;
            _commands = interactionService;
            _services = serviceProvider;

            await _commands.AddModulesAsync(typeof(IDiscordClientService).Assembly, _services);

            _client.InteractionCreated += HandleInteraction;
            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private async Task ComponentCommandExecuted(ComponentCommandInfo componentCommandInfo,
            IInteractionContext interactionContext, IResult result)
        {
            var componentCmd = (SocketMessageComponent) interactionContext.Interaction;

            _logger.LogInformation(
                "User {Username} ({UserId}) executed a component with id {CustomId}",
                componentCmd.User.Username, componentCmd.User.Id, componentCmd.Data.CustomId);

            if (result.IsSuccess is false)
            {
                if (result.Error is InteractionCommandError.Exception)
                {
                    await HandleExceptions(componentCmd, (ExecuteResult) result);
                }
                else
                {
                    await componentCmd.DeferAsync(true);
                    await HandleError(componentCmd, result);
                }
            }
            else await Task.CompletedTask;
        }

        private async Task SlashCommandExecuted(SlashCommandInfo slashCommandInfo,
            IInteractionContext interactionContext, IResult result)
        {
            var slashCmd = (SocketSlashCommand) interactionContext.Interaction;

            _logger.LogInformation(
                "User {Username} ({UserId}) executed a slash command /{SlashCommandName}",
                slashCmd.User.Username, slashCmd.User.Id, slashCmd.Data.Name);

            if (result.IsSuccess is false)
            {
                if (result.Error is InteractionCommandError.Exception)
                {
                    await HandleExceptions(slashCmd, (ExecuteResult) result);
                }
                else
                {
                    await slashCmd.DeferAsync(true);
                    await HandleError(slashCmd, result);
                }
            }

            else await Task.CompletedTask;
        }

        private async Task HandleInteraction(SocketInteraction socketInteraction)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, socketInteraction);
                await _commands.ExecuteCommandAsync(ctx, _services);
            }
            catch (Exception)
            {
                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (socketInteraction.Type == InteractionType.ApplicationCommand)
                    await socketInteraction.GetOriginalResponseAsync()
                        .ContinueWith(async msg => await msg.Result.DeleteAsync());
            }
        }

        private async Task HandleExceptions(IDiscordInteraction interaction, ExecuteResult result)
        {
            var user = await _mediator.Send(new GetUserQuery((long) interaction.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ой, кажется что-то пошло не так...");

            switch (result.Exception)
            {
                case GameUserExpectedException gameUserExpectedException:
                {
                    embed
                        .WithDescription(
                            $"{interaction.User.Mention.AsGameMention(user.Title)}, {gameUserExpectedException.Message}")
                        .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ExpectedException)));

                    break;
                }

                default:
                {
                    embed
                        .WithDescription(
                            $"{interaction.User.Mention.AsGameMention(user.Title)}, " +
                            "произошло что-то необычное и я уже сообщила об этом команде разработки. " +
                            "Приношу извинения за моих глупых создателей, они обязательно исправятся.")
                        .WithImageUrl(
                            await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.NotExpectedException)));

                    _logger.LogError(result.Exception, "Interaction ended with unexpected exception");

                    break;
                }
            }

            await interaction.FollowupAsync(embed: embed.Build(), ephemeral: true);
        }

        private async Task HandleError(IDiscordInteraction interaction, IResult result)
        {
            var user = await _mediator.Send(new GetUserQuery((long) interaction.User.Id));

            var embed = new EmbedBuilder()
                .WithUserColor(user.CommandColor)
                .WithAuthor("Ой, кажется что-то пошло не так...")
                .WithDescription(
                    $"{interaction.User.Mention.AsGameMention(user.Title)}, {result.ErrorReason}")
                .WithImageUrl(await _mediator.Send(new GetImageUrlQuery(Data.Enums.Image.ExpectedException)));

            await interaction.FollowupAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}