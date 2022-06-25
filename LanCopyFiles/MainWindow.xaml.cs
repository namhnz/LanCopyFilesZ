using System;
using System.Windows;
using System.Windows.Threading;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;

namespace LanCopyFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileReceivingService _receiverService;

        public MainWindow()
        {
            InitializeComponent();

            // Kiem tra xem cac thuc muc send temp va receive temp da ton tai hay chua, neu chua co thi tao cac thu muc nay
            AppStorage.Instance.EnsureTempFoldersExist();

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

            // var desktopFileSystemNode = FileSystemStructureBuilder.GetStructure(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            // Trace.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(desktopFileSystemNode));
        }

        // Nguon: https://stackoverflow.com/q/2688923/7182661
        private void Window_Closed(object sender, EventArgs e)
        {
            // Xoa toan bo cac file trong cac thu muc send temp va receive temp
            AppStorage.Instance.ClearTempFolders();

            // Thoat hoan toan tat ca cac thread
            Environment.Exit(Environment.ExitCode);
        }
        
    }
}