using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LanCopyFiles.Services;
using LanCopyFiles.Services.FilePrepare;
using LanCopyFiles.Services.GetIPAddress;
using LanCopyFiles.Services.SendReceiveServices;
using log4net;

namespace LanCopyFiles.Pages
{
    /// <summary>
    /// Interaction logic for ReceiveFilesBoard.xaml
    /// </summary>
    public partial class ReceiveFilesBoard : Page
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly FileReceivingService _receiverService;

        public ReceiveFilesBoard()
        {
            InitializeComponent();

            receivingFileAnimationStackPanel.Visibility = Visibility.Collapsed;
            showAllIPAddressesStackPanel.Visibility = Visibility.Visible;

            InitIPAddressValues();

            _receiverService = FileReceivingService.Instance;

            _receiverService.DataStartReceivingOnServer += (sender, args) =>
            {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    () => { OnDataStartReceving(); }
                );
            };

            _receiverService.DataFinishReceivingOnServer += (sender, args) =>
            {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    () => OnDataFinishReceiving(args.ReceivingFileName)
                );
            };
        }

        // Nguon: https://stackoverflow.com/a/24320649/7182661
        private void DataTransferingGifMediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            dataTransferingGifMediaElement.Position = TimeSpan.FromMilliseconds(1);
            dataTransferingGifMediaElement.Play();
        }

        private void InitIPAddressValues()
        {
            // Dia chi IP uu tien nhat
            var currentConnectionIPAddress = GetCurrentConnectionIPAddress.GetIPv4();
            preferredIPAddressTextBlock.Text =
                $"Use this IP Address to receive files/folders: {currentConnectionIPAddress}:8085";

            // Toan bo dia chi IP khac
            var allIOnAllAdapters = GetIPAddressOnAllAdapters.GetAllIPv4().Except(new[] { currentConnectionIPAddress })
                .Select(x => $"{x}:8085");
            allIPAddressOnAllAdaperDisplayTextBlock.Text =
                "Other IP Addresses on this PC:\n" + string.Join("\n", allIOnAllAdapters.ToArray());
        }

        private void copyIPAddressButton_Click(object sender, RoutedEventArgs e)
        {
            var currentConnectionIPAddress = GetCurrentConnectionIPAddress.GetIPv4();

            Clipboard.SetText(currentConnectionIPAddress);
        }

        private void OnDataStartReceving()
        {
            try
            {
                receivingFileAnimationStackPanel.Visibility = Visibility.Visible;
                showAllIPAddressesStackPanel.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("An error has happened: " + ex.Message);
            }
        }

        private async void OnDataFinishReceiving(string receivingFileName)
        {
            try
            {
                await ExtractRemoveOrMoveReceivedFileToDesktop(receivingFileName);

                receivingFileAnimationStackPanel.Visibility = Visibility.Collapsed;
                showAllIPAddressesStackPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("An error has happened: " + ex.Message);
            }
        }

        private async Task ExtractRemoveOrMoveReceivedFileToDesktop(string receivedFileName)
        {
            try
            {
                // Neu ten file la duong dan day du thi xoa phan duong dan
                if (receivedFileName.StartsWith(TempFolderNames.ReceiveTempFolder))
                {
                    receivedFileName =
                        receivedFileName.Substring(TempFolderNames.ReceiveTempFolder.Length +
                                                   1); // Cong them 1 la do co dau \
                }

                var receivedFilePath = Path.Combine(TempFolderNames.ReceiveTempFolder, receivedFileName);

                await Task.Run(() =>
                {
                    var isFileBeFolderAtSource = Path.GetExtension(receivedFileName) == ".zip";
                    if (isFileBeFolderAtSource)
                    {
                        if (FileExtractor.IsFolderAlreadyExistOnDesktop(receivedFileName))
                        {
                            var replaceResult = MessageBox.Show(
                                "There is a folder with the same name already exist on Desktop, do you want to replace that folder?",
                                "Folder already exist", MessageBoxButton.YesNo);
                            if (replaceResult == MessageBoxResult.Yes)
                            {
                                FileExtractor.ExtractFolderCopied(receivedFileName);
                            }
                        }

                        File.Delete(receivedFilePath);
                        // Trace.WriteLine("Da giai nen folder ra desktop");
                        Log.Info("Da giai nen folder ra desktop");
                    }
                    else
                    {
                        var moveToDesktopFilePath =
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), receivedFileName);
                        if (File.Exists(moveToDesktopFilePath))
                        {
                            var replaceResult = MessageBox.Show(
                                "There is a file with the same name already exist on Desktop, do you want to replace that file?",
                                "Folder already exist", MessageBoxButton.YesNo);
                            if (replaceResult == MessageBoxResult.Yes)
                            {
                                File.Delete(moveToDesktopFilePath);
                                File.Move(receivedFilePath, moveToDesktopFilePath);
                            }
                        }
                        else
                        {
                            File.Move(receivedFilePath, moveToDesktopFilePath);
                        }

                        // Trace.WriteLine("Da di chuyen file ra desktop");
                        Log.Info("Da di chuyen file ra desktop");
                    }

                    // Received file/folder successfully
                    ShowReceivedThingSnackbar(receivedFileName);
                });
            }
            catch (Exception ex)
            {
                
                Log.Error(ex);
                MessageBox.Show("An error has happened: " + ex.Message);
            }
        }

        #region Hien thi thong bao

        private void ShowReceivedThingSnackbar(string receivedThingName)
        {
            (Application.Current.MainWindow as MainWindow)?.RootSnackbar.Show(
                "Just receive a file/folder from another PC!",
                $"The file/folder named {receivedThingName} was arrived, you can check it on Desktop");
        }


        #endregion
    }
}