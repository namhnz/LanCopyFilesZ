using System.Windows;
using System.Windows.Input;
using LanCopyFiles.Services.SendReceiveLogServices;
using LanCopyFiles.TransferFilesEngine;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;

namespace LanCopyFiles;

public class Bootstrapper : PrismBootstrapper
{
    override protected void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<ISendReceiveLogDataService, SendReceiveLogDataService>();
    }

    override protected DependencyObject CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    override protected void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<TransferFilesEngineModule>();
    }


    protected override void InitializeModules()
    {
    }
}