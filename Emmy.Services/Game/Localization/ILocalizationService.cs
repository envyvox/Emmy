using Emmy.Data.Enums;

namespace Emmy.Services.Game.Localization
{
    public interface ILocalizationService
    {
        string Localize(LocalizationCategory category, string keyword, uint amount = 1);
    }
}
