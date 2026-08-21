namespace RapWay.Presentation.Unity.Localization
{
    public interface IGameLocalizationService
    {
        string Get(LocalizationKey key);

        string Get(LocalizationKey key, params LocalizationArgument[] arguments);
    }
}
