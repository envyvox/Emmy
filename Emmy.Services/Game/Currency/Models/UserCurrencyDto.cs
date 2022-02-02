using AutoMapper;
using Emmy.Data.Entities.User;

namespace Emmy.Services.Game.Currency.Models
{
    public record UserCurrencyDto(
        Data.Enums.Currency Type,
        uint Amount);

    public class UserCurrencyProfile : Profile
    {
        public UserCurrencyProfile() => CreateMap<UserCurrency, UserCurrencyDto>();
    }
}