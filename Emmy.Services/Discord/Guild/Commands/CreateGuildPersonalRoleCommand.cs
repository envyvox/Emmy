using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using static Emmy.Data.Enums.Discord.Role;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record CreateGuildPersonalRoleCommand(string Name, string Color) : IRequest<ulong>;

    public class CreateGuildPersonalRoleHandler : IRequestHandler<CreateGuildPersonalRoleCommand, ulong>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateGuildPersonalRoleHandler> _logger;

        public CreateGuildPersonalRoleHandler(
            IMediator mediator,
            ILogger<CreateGuildPersonalRoleHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ulong> Handle(CreateGuildPersonalRoleCommand request, CancellationToken ct)
        {
            var roles = DiscordRepository.Roles;
            var guild = await _mediator.Send(new GetSocketGuildQuery());

            try
            {
                var created = await guild.CreateRoleAsync(
                    name: request.Name,
                    permissions: null,
                    color: new Color(uint.Parse(request.Color, NumberStyles.HexNumber)),
                    isHoisted: false,
                    options: null);

                await created.ModifyAsync(x => x.Position = guild.GetRole(roles[Premium].Id).Position + 1);

                _logger.LogInformation(
                    "Created socket guild role {@Role}",
                    created);

                return created.Id;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot create guild role with name {Name} and color {Color}",
                    request.Name, request.Color);
            }

            return 0;
        }
    }
}