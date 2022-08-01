using System;
using LanCopyFiles.Events;
using LanCopyFiles.Pages.Views;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;
using Prism.Events;
using Prism.Mvvm;
using Wpf.Ui.Mvvm.Contracts;

namespace LanCopyFiles;

public class MainWindowViewModel : BindableBase
{
    private readonly INavigationService _navigationService;
    private readonly IAppStorage _appStorage;
    private readonly IFileReceivingService _fileReceivingService;
    private readonly IEventAggregator _eventAggregator;

    public MainWindowViewModel(INavigationService navigationService, IAppStorage appStorage,
        IFileReceivingService fileReceivingService, IEventAggregator eventAggregator)
    {
        _navigationService = navigationService;
        _appStorage = appStorage;
        _fileReceivingService = fileReceivingService;
        _eventAggregator = eventAggregator;

        Init();
    }

    private void Init()
    {
        // Kiem tra xem cac thuc muc send temp va receive temp da ton tai hay chua, neu chua co thi tao cac thu muc nay
        _appStorage.EnsureTempFoldersExist();
        _appStorage.ClearTempFolders();

        // _fileReceivingService.DataStartReceivingOnServer += (sender, args) =>
        // {
        //     // // Nguon: https://stackoverflow.com/a/21306951/7182661
        //     //
        //     // Dispatcher.BeginInvoke(
        //     //     DispatcherPriority.Background,
        //     //     new Action(() => RootNavigation.Navigate(("receive-data-page")))
        //     // );
        //     _navigationService.Navigate(typeof(ReceiveFilesBoard));
        // };

        _eventAggregator.GetEvent<DataStartReceivingOnServerEvent>().Subscribe(_ =>
        {
            _navigationService.Navigate(typeof(ReceiveFilesBoard));
        });

        _fileReceivingService.StartService();
    }

    public void Close()
    {
        // Xoa toan bo cac file trong cac thu muc send temp va receive temp
        _appStorage.ClearTempFolders();

        // Thoat hoan toan tat ca cac thread
        Environment.Exit(Environment.ExitCode);
    }
}