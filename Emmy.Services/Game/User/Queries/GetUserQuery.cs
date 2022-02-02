using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Services.Discord.Guild.Queries;
using Emmy.Services.Game.User.Commands;
using Emmy.Services.Game.User.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Services.Game.User.Queries
{
    public record GetUserQuery(long UserId) : IRequest<UserDto>;

    public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetUserHandler(
            DbContextOptions options,
            IMapper mapper,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken ct)
        {
            var entity = await _db.Users
                .SingleOrDefaultAsync(x => x.Id == request.UserId);

            if (entity is null)
            {
                var socketUser = await _mediator.Send(new GetSocketGuildUserQuery((ulong) request.UserId));

                if (socketUser.IsBot)
                {
                    throw new Exception(
                        $"user {request.UserId} are bot");
                }

                return await _mediator.Send(new CreateUserCommand(request.UserId));
            }

            return _mapper.Map<UserDto>(entity);
        }
    }
}