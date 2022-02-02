using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Emmy.Services.Discord.Client.Events;
using Emmy.Services.Discord.Client.Events.Message;
using Emmy.Services.Discord.Client.Events.Reaction;
using Emmy.Services.Discord.Client.Events.ScheduledEvent;
using Emmy.Services.Discord.Client.Events.User;
using Emmy.Services.Discord.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Emmy.Services.Discord.Client
{
    public class DiscordClientService : IDiscordClientService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMediator _mediator;
        private readonly DiscordClientOptions _options;
        private DiscordSocketClient _socketClient;
        private InteractionService _interactionService;

        public DiscordClientService(
            IOptions<DiscordClientOptions> options,
            IServiceProvider serviceProvider,
            IMediator mediator)
        {
            _serviceProvider = serviceProvider;
            _mediator = mediator;
            _options = options.Value;
        }

        public async Task Start()
        {
            _socketClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100,
                AlwaysDownloadUsers = true,
                GatewayIntents =
                    GatewayIntents.Guilds |
                    GatewayIntents.GuildMembers |
                    GatewayIntents.GuildMessageReactions |
                    GatewayIntents.GuildMessages |
                    GatewayIntents.GuildVoiceStates |
                    GatewayIntents.GuildScheduledEvents
            });
            _interactionService = new InteractionService(_socketClient.Rest);

            await _serviceProvider.GetRequiredService<CommandHandler>()
                .InitializeAsync(_socketClient, _interactionService, _serviceProvider);

            await _socketClient.LoginAsync(TokenType.Bot, _options.Token);
            await _socketClient.StartAsync();

            _socketClient.Log += ClientOnLog;
            _socketClient.Ready += ClientOnReady;
            _socketClient.MessageReceived += ClientOnMessageReceived;
            _socketClient.MessageDeleted += ClientOnMessageDeleted;
            _socketClient.MessageUpdated += ClientOnMessageUpdated;
            _socketClient.ReactionAdded += ClientOnReactionAdded;
            _socketClient.ReactionRemoved += ClientOnReactionRemoved;
            _socketClient.UserVoiceStateUpdated += ClientOnUserVoiceStateUpdated;
            _socketClient.UserJoined += ClientOnUserJoined;
            _socketClient.UserLeft += ClientOnUserLeft;
            _socketClient.GuildScheduledEventStarted += ClientOnGuildScheduledEventStarted;
            _socketClient.GuildScheduledEventCreated += ClientOnGuildScheduledEventCreated;
            _socketClient.GuildScheduledEventCancelled += ClientOnGuildScheduledEventCancelled;
            _socketClient.GuildScheduledEventCompleted += ClientOnGuildScheduledEventCompleted;
            _socketClient.GuildScheduledEventUpdated += ClientOnGuildScheduledEventUpdated;
            _socketClient.GuildScheduledEventUserAdd += ClientOnGuildScheduledEventUserAdd;
            _socketClient.GuildScheduledEventUserRemove += ClientOnGuildScheduledEventUserRemove;
            _interactionService.Log += ClientOnLog;
        }

        public async Task<DiscordSocketClient> GetSocketClient()
        {
            return await Task.FromResult(_socketClient);
        }

        private async Task ClientOnGuildScheduledEventUserRemove(
            Cacheable<SocketUser, RestUser, IUser, ulong> socketUser,
            SocketGuildEvent socketGuildEvent)
        {
            await _mediator.Send(new OnGuildScheduledEventUserRemove(socketUser, socketGuildEvent));
        }

        private async Task ClientOnGuildScheduledEventUserAdd(Cacheable<SocketUser, RestUser, IUser, ulong> socketUser,
            SocketGuildEvent socketGuildEvent)
        {
            await _mediator.Send(new OnGuildScheduledEventUserAdd(socketUser, socketGuildEvent));
        }

        private async Task ClientOnGuildScheduledEventUpdated(Cacheable<SocketGuildEvent, ulong> oldSocketGuildEvent,
            SocketGuildEvent newSocketGuildEvent)
        {
            await _mediator.Send(new OnGuildScheduledEventUpdated(oldSocketGuildEvent, newSocketGuildEvent));
        }

        private async Task ClientOnGuildScheduledEventCompleted(SocketGuildEvent socketGuildEvent)
        {
            await _mediator.Send(new OnGuildScheduledEventCompleted(socketGuildEvent));
        }

        private async Task ClientOnGuildScheduledEventCancelled(SocketGuildEvent socketGuildEvent)
        {
            await _mediator.Send(new OnGuildScheduledEventCancelled(socketGuildEvent));
        }

        private async Task ClientOnGuildScheduledEventCreated(SocketGuildEvent socketGuildEvent)
        {
            await _mediator.Send(new OnGuildScheduledEventCreated(socketGuildEvent));
        }

        private async Task ClientOnGuildScheduledEventStarted(SocketGuildEvent socketGuildEvent)
        {
            await _mediator.Send(new OnGuildScheduledEventStarted(socketGuildEvent));
        }

        private async Task ClientOnUserLeft(SocketGuild socketGuild, SocketUser socketUser)
        {
            await _mediator.Send(new OnUserLeft(socketGuild, socketUser));
        }

        private async Task ClientOnUserJoined(SocketGuildUser socketGuildUser)
        {
            await _mediator.Send(new OnUserJoined(socketGuildUser));
        }

        private async Task ClientOnUserVoiceStateUpdated(SocketUser socketUser, SocketVoiceState oldSocketVoiceState,
            SocketVoiceState newSocketVoiceState)
        {
            await _mediator.Send(new OnUserVoiceStateUpdated(socketUser, oldSocketVoiceState, newSocketVoiceState));
        }

        private async Task ClientOnReactionRemoved(Cacheable<IUserMessage, ulong> userMessage,
            Cacheable<IMessageChannel, ulong> messageChannel, SocketReaction socketReaction)
        {
            await _mediator.Send(new OnReactionRemoved(userMessage, messageChannel, socketReaction));
        }

        private async Task ClientOnReactionAdded(Cacheable<IUserMessage, ulong> userMessage,
            Cacheable<IMessageChannel, ulong> messageChannel, SocketReaction socketReaction)
        {
            await _mediator.Send(new OnReactionAdded(userMessage, messageChannel, socketReaction));
        }

        private async Task ClientOnMessageUpdated(Cacheable<IMessage, ulong> oldMessage, SocketMessage newMessage,
            ISocketMessageChannel socketMessageChannel)
        {
            await _mediator.Send(new OnMessageUpdated(oldMessage, newMessage, socketMessageChannel));
        }

        private async Task ClientOnMessageDeleted(Cacheable<IMessage, ulong> deletedMessage,
            Cacheable<IMessageChannel, ulong> messageChannel)
        {
            await _mediator.Send(new OnMessageDeleted(deletedMessage, messageChannel));
        }

        private async Task ClientOnMessageReceived(SocketMessage socketMessage)
        {
            await _mediator.Send(new OnMessageReceived(socketMessage));
        }

        private async Task ClientOnReady()
        {
            await _mediator.Send(new OnReady(_interactionService));
        }

        private async Task ClientOnLog(LogMessage logMessage)
        {
            await _mediator.Send(new OnLog(logMessage));
        }
    }
}