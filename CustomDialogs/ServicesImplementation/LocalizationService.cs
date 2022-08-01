using CustomDialogs.Services;

namespace CustomDialogs.ServicesImplementation
{
    internal sealed class LocalizationService : ILocalizationService
    {
        public string LocalizeFromResourceKey(string resourceKey)
        {
            return resourceKey.GetLocalized();
        }
    }
}
