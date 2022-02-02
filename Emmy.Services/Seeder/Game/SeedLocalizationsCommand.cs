using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Emmy.Services.Game.Localization.Commands;
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