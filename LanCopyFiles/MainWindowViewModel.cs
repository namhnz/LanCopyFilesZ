using System;
using System.Windows.Threading;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;
using Prism.Mvvm;

namespace LanCopyFiles;

public class MainWindowViewModel: BindableBase
{
    private readonly IAppStorage _appStorage;
    private readonly FileReceivingService _receiverService;

    public MainWindowViewModel(IAppStorage appStorage)
    {
        _appStorage = appStorage;

        Init();
    }

    private void Init()
    {
        // Kiem tra xem cac thuc muc send temp va receive temp da ton tai hay chua, neu chua co thi tao cac thu muc nay
        _appStorage.EnsureTempFoldersExist();
        _appStorage.ClearTempFolders();

        _receiverService = FileReceivingService.Instance;
        _receiverService.DataStartReceivingOnServer += (sender, args) =>
        {
            // Nguon: https://stackoverflow.com/a/21306951/7182661

            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => RootNavigation.Navigate(("receive-data-page")))
            );
        };

        _receiverService.StartService();
    }

    public void Close()
    {
        // Xoa toan bo cac file trong cac thu muc send temp va receive temp
        _appStorage.ClearTempFolders();

        // Thoat hoan toan tat ca cac thread
        Environment.Exit(Environment.ExitCode);
    }
}