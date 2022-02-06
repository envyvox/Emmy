using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emmy.Data;
using Emmy.Data.Enums;
using Emmy.Data.Extensions;
using Emmy.Services.Game.Banner.Commands;
using Emmy.Services.Game.Banner.Queries;
using Emmy.Services.Game.Currency.Commands;
using Emmy.Services.Game.User.Models;
using Emmy.Services.Game.World.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EmbedBuilderExtensions = Emmy.Services.Extensions.EmbedBuilderExtensions;

namespace Emmy.Services.Game.User.Commands
{
    public record CreateUserCommand(long UserId) : IRequest<UserDto>;

    public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly ILogger<CreateUserHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public CreateUserHandler(
            DbContextOptions options,
            ILogger<CreateUserHandler> logger,
            IMapper mapper,
            IMediator mediator)
        {
            _db = new AppDbContext(options);
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
        {
            var exist = await EntityFrameworkQueryableExtensions.AnyAsync(_db.Users, x => x.Id == request.UserId);

            if (exist)
            {
                throw new Exception(
                    $"user {request.UserId} already exist");
            }

            var entity = await _db.CreateEntity(new Data.Entities.User.User
            {
                Id = request.UserId,
                About = null,
                Level = 1,
                Xp = 0,
                Fraction = Fraction.Undefined,
                Location = Location.Neutral,
                Title = Title.Newbie,
                Gender = Gender.None,
                CubeType = CubeType.D6,
                CommandColor = EmbedBuilderExtensions.DefaultEmbedColor,
                IsPremium = false,
                OnGuild = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            var banners = await _mediator.Send(new GetBannersQuery());
            var banner = banners.Single(x => x.Name == "Ночной город");
            var currencyAmount = await _mediator.Send(new GetWorldPropertyValueQuery(
                WorldProperty.EconomyStartupCapital));

            await _mediator.Send(new AddBannerToUserCommand(
                request.UserId, banner.Id, true));
            await _mediator.Send(new AddCurrencyToUserCommand(
                request.UserId, Data.Enums.Currency.Token, currencyAmount));

            _logger.LogInformation(
                "Created user entity for user {UserId}",
                request.UserId);

            return _mapper.Map<UserDto>(entity);
        }
    }
}