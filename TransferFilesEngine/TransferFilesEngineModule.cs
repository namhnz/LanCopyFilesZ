using Prism.Ioc;
using Prism.Modularity;

namespace LanCopyFiles.TransferFilesEngine;

public class TransferFilesEngineModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
        
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {

    }
}