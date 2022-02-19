using System;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Services.Discord.Guild.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Emmy.Services.Discord.Guild.Commands
{
    public record RenameGuildUserCommand(ulong UserId, string Nickname) : IRequest;

    public class RenameGuildUserHandler : IRequestHandler<RenameGuildUserCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RenameGuildUserHandler> _logger;

        public RenameGuildUserHandler(
            IMediator mediator,
            ILogger<RenameGuildUserHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(RenameGuildUserCommand request, CancellationToken cancellationToken)
        {
            var socketGuildUser = await _mediator.Send(new GetSocketGuildUserQuery(request.UserId));

            try
            {
                await socketGuildUser!.ModifyAsync(x => x.Nickname = request.Nickname);

                _logger.LogInformation(
                    "Updated user {UserId} nickname to {Nickname}",
                    request.UserId, request.Nickname);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Can't modify user {UserId} nickname to {Nickname}",
                    request.UserId, request.Nickname);
            }

            return new Unit();
        }
    }
}