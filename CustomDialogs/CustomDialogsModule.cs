using CustomDialogs.Services;
using CustomDialogs.ServicesImplementation;
using Prism.Ioc;
using Prism.Modularity;

namespace CustomDialogs;

public class CustomDialogsModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<ILocalizationService, LocalizationService>();
        
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {

    }
}