#nullable enable

using CustomDialogs.Services;
using Prism.Ioc;

namespace CustomDialogs.Extensions
{
    public static class LocalizationExtensions
    {
        private static ILocalizationService? FallbackLocalizationService;

        public static string ToLocalized(this string resourceKey, ILocalizationService? localizationService = null)
        {
            if (localizationService == null)
            {
                FallbackLocalizationService ??= ContainerLocator.Container.Resolve<ILocalizationService>();
                return FallbackLocalizationService?.LocalizeFromResourceKey(resourceKey) ?? string.Empty;
            }

            return localizationService.LocalizeFromResourceKey(resourceKey);
        }
    }
}