using System.Windows;
using System.Windows.Input;
using LanCopyFiles.Configs;
using LanCopyFiles.Services.PackingServices;
using LanCopyFiles.Services.SendReceiveLogServices;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;
using LanCopyFiles.Services.StorageServices.FilePrepare;
using LanCopyFiles.TransferFilesEngine;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace LanCopyFiles;

public class Bootstrapper : PrismBootstrapper
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Khoi tao cac service cua WPFUI
        containerRegistry.RegisterSingleton<ISnackbarService, SnackbarService>();
        containerRegistry.RegisterSingleton<INavigationService, NavigationService>();

        // Khoi tao cac service cua app
        containerRegistry.RegisterSingleton<IGlobalAppConfigs, GlobalAppConfigs>();

        containerRegistry.RegisterSingleton<ISendingTempFolder, SendingTempFolder>();
        containerRegistry.RegisterSingleton<IReceivingTempFolder, ReceivingTempFolder>();
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