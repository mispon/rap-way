using RapWay.Domain.Localization;

namespace RapWay.Core.Localization
{
    public interface IGameLocalizationService
    {
        string Get(LocalizationKey key);

        string Get(LocalizationKey key, params LocalizationArgument[] arguments);
    }
}
