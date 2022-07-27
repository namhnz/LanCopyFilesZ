using System.Windows;
using System.Windows.Input;
using LanCopyFiles.Services.PackingServices;
using LanCopyFiles.Services.SendReceiveLogServices;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;
using LanCopyFiles.TransferFilesEngine;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;

namespace LanCopyFiles;

public class Bootstrapper : PrismBootstrapper
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {

        containerRegistry.RegisterSingleton<IAppStorage, AppStorage>();
        containerRegistry.RegisterSingleton<IZipService, ZipService>();
        containerRegistry.RegisterSingleton<IFileSendingService, FileSendingService>();
        containerRegistry.RegisterSingleton<IFileReceivingService, FileReceivingService>();
        containerRegistry.RegisterSingleton<ISendReceiveLogDataService, SendReceiveLogDataService>();
    }

    protected override DependencyObject CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<TransferFilesEngineModule>();
    }


    protected override void InitializeModules()
    {
    }
}