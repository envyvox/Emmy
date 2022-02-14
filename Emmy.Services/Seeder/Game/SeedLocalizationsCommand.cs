using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Emmy.Services.Game.Crop.Queries;
using Emmy.Services.Game.Fish.Queries;
using Emmy.Services.Game.Localization.Commands;
using Emmy.Services.Game.Seed.Queries;
using MediatR;

namespace Emmy.Services.Seeder.Game
{
    public record SeedLocalizationsCommand : IRequest<TotalAndAffectedCountDto>;

    public class SeedLocalizationsHandler : IRequestHandler<SeedLocalizationsCommand, TotalAndAffectedCountDto>
    {
        private readonly IMediator _mediator;

        public SeedLocalizationsHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TotalAndAffectedCountDto> Handle(SeedLocalizationsCommand request, CancellationToken ct)
        {
            var result = new TotalAndAffectedCountDto();
            var commands = new List<CreateLocalizationCommand>();

            foreach (var category in Enum
                .GetValues(typeof(LocalizationCategory))
                .Cast<LocalizationCategory>())
            {
                switch (category)
                {
                    case LocalizationCategory.Basic:

                        commands.Add(new CreateLocalizationCommand(
                            category, "User", "пользователь", "пользователя", "пользователей"));
                        commands.Add(new CreateLocalizationCommand(
                            category, "Message", "сообщение", "сообщения", "сообщений"));
                        commands.Add(new CreateLocalizationCommand(
                            category, "Post", "публикация", "публикации", "публикаций"));
                        commands.Add(new CreateLocalizationCommand(
                            category, "Ruble", "рубль", "рубля", "рублей"));
                        commands.Add(new CreateLocalizationCommand(
                            category, "FractionPoints", "очко прогресса", "очка прогресса", "очков прогресса"));

                        break;
                    case LocalizationCategory.Vote:

                        commands.Add(new CreateLocalizationCommand(
                            category, Vote.Like.ToString(), "лайк", "лайка", "лайков"));
                        commands.Add(new CreateLocalizationCommand(
                            category, Vote.Dislike.ToString(), "дизлайк", "дизлайка", "дизлайков"));

                        break;
                    case LocalizationCategory.Currency:

                        commands.Add(new CreateLocalizationCommand(
                            category, Currency.Token.ToString(), "токен", "токена", "токенов"));
                        commands.Add(new CreateLocalizationCommand(
                            category, Currency.Lobbs.ToString(), "лоббс", "лоббса", "лоббсов"));

                        break;
                    case LocalizationCategory.Key:

                        commands.Add(new CreateLocalizationCommand(
                            category, KeyType.LoveRoom.ToString(), "ключ от любовного гнезда", "ключа от любовного гнезда", "ключей от любовного гнезда"));
                        commands.Add(new CreateLocalizationCommand(
                            category, KeyType.PrivateRoom.ToString(), "ключ от приватного сектора", "ключа от приватного сектора", "ключей от приватного сектора"));

                        break;
                    case LocalizationCategory.Container:

                        commands.Add(new CreateLocalizationCommand(
                            category, Container.Token.ToString(), "контейнер с токенами", "контейнера с токенами", "контейнеров с токенами"));
                        commands.Add(new CreateLocalizationCommand(
                            category, Container.Supply.ToString(), "контейнер с припасами", "контейнера с припасами", "контейнеров с припасами"));

                        break;
                    case LocalizationCategory.Fish:

                        var fishes = await _mediator.Send(new GetFishesQuery());

                        commands.AddRange(fishes.Select(fish => new CreateLocalizationCommand(
                            category, fish.Name, fish.Name, fish.Name, fish.Name)));

                        break;
                    case LocalizationCategory.Crop:

                        var crops = await _mediator.Send(new GetCropsQuery());

                        commands.AddRange(crops.Select(crop => new CreateLocalizationCommand(
                            category, crop.Name, crop.Name, crop.Name, crop.Name)));

                        break;
                    case LocalizationCategory.Seed:

                        var seeds = await _mediator.Send(new GetSeedsQuery());

                        commands.AddRange(seeds.Select(seed => new CreateLocalizationCommand(
                            category, seed.Name, seed.Name, seed.Name, seed.Name)));

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var command in commands)
            {
                result.Total++;

                try
                {
                    await _mediator.Send(command);

                    result.Affected++;
                }
                catch
                {
                    // ignored
                }
            }

            return result;
        }
    }
}