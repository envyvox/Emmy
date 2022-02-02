using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using MediatR;
using IRequest = MediatR.IRequest;

namespace Emmy.Services.Discord.Embed
{
    public record FollowUpEmbedCommand(
            SocketInteraction Interaction,
            EmbedBuilder EmbedBuilder,
            MessageComponent Components = null,
            string Text = null,
            bool Ephemeral = false)
        : IRequest;

    public class FollowUpEmbedHandler : IRequestHandler<FollowUpEmbedCommand>
    {
        public async Task<Unit> Handle(FollowUpEmbedCommand request, CancellationToken ct)
        {
            try
            {
                await request.Interaction.FollowupAsync(request.Text,
                    embed: request.EmbedBuilder.Build(),
                    components: request.Components,
                    ephemeral: request.Ephemeral);
            }
            catch (HttpException e)
            {
                foreach (var discordJsonError in e.Errors)
                {
                    foreach (var discordError in discordJsonError.Errors)
                    {
                        Console.WriteLine(discordError.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Unit.Value;
        }
    }
}