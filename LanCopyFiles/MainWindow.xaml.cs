using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EasyFileTransfer;
using LanCopyFiles.Services;
using LanCopyFiles.Services.FilePrepare;
using LanCopyFiles.Services.FileSystemAnalyze;

namespace LanCopyFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ThingReceiverService _receiverService;

        public MainWindow()
        {
            InitializeComponent();

            FilePacker.EnsureSendTempFolderExist();
            FileExtractor.EnsureReceiveTempFolderExist();

            _receiverService = ThingReceiverService.Instance;
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
            // Xoa toan bo cac file trong thu muc temp
            AppTempFolder.DeleteAllFilesInTempFolder();

            // Thoat hoan toan tat ca cac thread
            Environment.Exit(Environment.ExitCode);
        }
        
    }
}