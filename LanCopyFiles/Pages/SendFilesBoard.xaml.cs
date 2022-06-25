using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using LanCopyFiles.Configs;
using LanCopyFiles.Extensions;
using LanCopyFiles.Models;
using LanCopyFiles.Services;
using LanCopyFiles.Services.IPAddressManager;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;
using LanCopyFiles.Services.StorageServices.FilePrepare;
using log4net;
using Ookii.Dialogs.Wpf;
using SharpConfig;

namespace LanCopyFiles.Pages
{
    /// <summary>
    /// Interaction logic for SendFilesBoard.xaml
    /// </summary>
    public partial class SendFilesBoard : Page
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SendFilesBoard()
        {
            InitializeComponent();

            LoadConfigs();

            _filesOrFoldersPickerContainerClickWaitTimer =
                new DispatcherTimer(
                    new TimeSpan(0, 0, 0, 0, 400),
                    DispatcherPriority.Background,
                    MouseWaitTimer_Tick,
                    Dispatcher.CurrentDispatcher);

            // InitUpdateProgressBarTimer();
        }

        #region Phan thuc thi khi drag/drop cac file hoac folder vao panel

        // Nguon: https://stackoverflow.com/a/5663329/7182661
        private async void FilesPickerContainerStackPanel_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                // HandleFileOpen(files[0]);

                await StartSendingProcess(files);
            }
        }

        #endregion

        #region Su dung de phan biet click va double click

        // Nguon: https://stackoverflow.com/a/971676/7182661
        private DispatcherTimer _filesOrFoldersPickerContainerClickWaitTimer;

        private bool _isFilesOrFoldersPickerContainerDirty = false;

        // Click 1 lan chuot de chon cac file can copy, khong co thu muc
        private async void MouseWaitTimer_Tick(object sender, EventArgs e)
        {
            _filesOrFoldersPickerContainerClickWaitTimer.Stop();

            // Kiem tra xem co phai lan chay dau tien hay khong
            if (!_isFilesOrFoldersPickerContainerDirty)
            {
                _isFilesOrFoldersPickerContainerDirty = true;
                return;
            }

            // Handle Single Click Actions
            Trace.WriteLine("Single Click");

            // Do chi su dung 1 click nen hien thi hop thoai chon cac file
            await ShowFilesSelectDialog();
        }

        #endregion

        #region Phan thuc thi khi click/double click vao panel de lua chon cac file hoac folder

        private void FilesPickerCardAction_OnClick(object sender, RoutedEventArgs e)
        {
            _filesOrFoldersPickerContainerClickWaitTimer.Start();
        }

        // Click 2 lan chuot de chon cac folder can copy, khong co file
        private void FilesPickerCardAction_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Stop the timer from ticking.
            _filesOrFoldersPickerContainerClickWaitTimer.Stop();

            Trace.WriteLine("Double Click");
            e.Handled = true;

            // Nhan dup chuot nen hien thi hop thoai lua chon cac folder
            ShowFoldersSelectDialog();
        }

        #endregion

        #region Hien thi dialog lua chon chi cac file

        private async Task ShowFilesSelectDialog()
        {
            var filesSelectDialog = new VistaOpenFileDialog();
            filesSelectDialog.Multiselect = true;

            var selectResult = filesSelectDialog.ShowDialog();

            if (selectResult ?? false)
            {
                var filePaths = filesSelectDialog.FileNames;

                await StartSendingProcess(filePaths);
            }
        }

        #endregion

        #region Hien thi dialog lua chon chi cac thu muc

        private async Task ShowFoldersSelectDialog()
        {
            var foldersSelectDialog = new VistaFolderBrowserDialog();
            foldersSelectDialog.Multiselect = true;

            var selectResult = foldersSelectDialog.ShowDialog();

            if (selectResult ?? false)
            {
                var folderPaths = foldersSelectDialog.SelectedPaths;

                await StartSendingProcess(folderPaths);
            }
        }

        #endregion


        #region Phan thuc thi khi su dung chuot phai copy/paste vao panel 

        private async void FilesOrFoldersPickerContextMenuPasteItem_OnClick(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("Pasting");
            // Nguon: https://stackoverflow.com/a/68001651/7182661
            if (Clipboard.ContainsFileDropList())
            {
                var filesArray = Clipboard.GetFileDropList();
                //now you have a array of file address 
                // MessageBox.Show(string.Join("|", filesArray.Cast<string>().ToList()));

                await StartSendingProcess(filesArray.Cast<string>().ToArray());
            }
            else if (Clipboard.ContainsText())
            {
                var fileName = Clipboard.GetText();
            }
        }

        #endregion


        #region Gui cac file den server

        private async Task<List<bool>> SendFilesToDestinationPC(string[] thingPaths, string destinationPCIPAddress)
        {
            // Hien thi thong bao trang thai dang ket noi den dia chi IP
            SendingStatusTextBlock.Text = $"Connecting to the destination PC's IP address: {destinationPCIPAddress}";
            
            // Kiem tra xem co ket noi duoc den server khong
            await Task.Run(() =>
            {
                if (!IPAddressValidator.TestConnectionUsingPingHost(destinationPCIPAddress))
                {
                    throw new WebException($"Can't connect to {destinationPCIPAddress}");
                }
            });

            // Hien thi thong bao trang thai ket noi thanh cong
            SendingStatusTextBlock.Text = $"Connected to the destination PC: {destinationPCIPAddress}";

            // Luu lai dia chi IP trong truong hop kiem tra ket noi thanh cong
            GlobalAppConfigs.Instance.SendFilesConfigs.SetSavedDestinationPCIPAddressConfigValue(destinationPCIPAddress);
            
            // Hien thi so luong file va folder se copy
            SendingStatusTextBlock.Text = $"There will be {thingPaths.Length} file(s) and folder(s) sent";
            

            var sendResultTask = await Task.Run(() =>
            {
                var fileSendingService = new FileSendingService(thingPaths, destinationPCIPAddress);
                var sendFileResults = fileSendingService.SendFilesToDestinationPC();

                return sendFileResults;
            });

            ClearSendingProgressStatus();

            return sendResultTask;
        }

        private void ClearSendingProgressStatus()
        {
            SendingProgressBar.SetPercent(0);
            SendingStatusTextBlock.Text = string.Empty;
        }

        private void OnSendingProgressChanged(object sender, FilesSendingProgressInfoArgs args)
        {
            SendingProgressBar.SetPercent(args.TotalSendingPercentage);
            SendingStatusTextBlock.Text =
                $"Transferring file {args.SendingFileName}: {Math.Ceiling(SendingProgressBar.Value)}%";

            Trace.WriteLine($"Dang gui file :{SendingProgressBar.Value}%");
        }
        
        #endregion

        #region Luu lai cac cai dat

        // private Section _ipCopyToSectionConfig;
        // private Configuration _config;


        private void LoadConfigs()
        {
            var destinationPCIPAddress =
                GlobalAppConfigs.Instance.SendFilesConfigs.GetSavedDestinationPCIPAddressConfigValue();

            if (!string.IsNullOrEmpty(destinationPCIPAddress))
            {
                destinationPCIPAddressTextBox.Text = destinationPCIPAddress;
            }
            
        }
        
        #endregion
        
        #region Copy for all

        public async Task StartSendingProcess(string[] thingPaths)
        {
            try
            {
                // Kiem tra xem thu dia chi IP may dich da day du hay chua
                var destinationPCIPAddress = destinationPCIPAddressTextBox.Text;

                if (!IPAddressValidator.CheckIfValidFormatIPv4Only(destinationPCIPAddress))
                {
                    OpenMessageBox("Invalid IP address", "The destination PC's IP address is invalid");
                    return;
                }

                // Disable panel truyen file trong khi dang chuyen file sang may khac
                FilesPickerCardAction.IsEnabled = false;

                // Chuan bi cac file can copy vao thu muc temp
                // Chinh thanh progress bar sang trang thai indetermine va status thanh prepare copying file(s)/folder(s)
                SendingProgressBar.IsIndeterminate = true;
                SendingStatusTextBlock.Text = "Getting ready to transfer file(s) or folder(s)";

                // Copy cac file va folder vao thu muc send temp
                await Task.Run(() =>
                {
                    AppStorage.Instance.SendingTempFolder.AddMany(thingPaths);
                    Trace.WriteLine("Da copy xong cac file vao thu muc temp");
                });

                SendingProgressBar.IsIndeterminate = false;
                SendingStatusTextBlock.Text = "The file(s)/folder(s) is/are ready for transfer";

                // Lay duong dan tat cac cac file da copy vao trong thu muc send temp
                var allFilesInTempFolder = AppStorage.Instance.SendingTempFolder.GetAll().ToArray();

                // Gui file den may dich
                var copyResults = await SendFilesToDestinationPC(allFilesInTempFolder, destinationPCIPAddress);

                // Dem so luong file va folder da gui thanh cong
                var thingsSendSuccessCount = copyResults.Count(x => x);

                // Hien thi ket qua
                ShowSnackbar("All the work has been completed!",
                    $"There are {thingsSendSuccessCount} file(s) or folder(s) that were successfully sent and {thingPaths.Length - thingsSendSuccessCount} that were not");

            }
            catch (Exception ex)
            {
                ClearSendingProgressStatus();
                OpenMessageBox("Sending failed", "An error has happened: " + ex.Message);
                
                Log.Error(ex);
            }
            finally
            {
                // Xoa toan bo file trong cac thu muc send-temp, receive-temp
                AppStorage.Instance.ClearTempFolders();

                // Enable lai panel truyen file trong sau khi da chuyen file sang may khac
                FilesPickerCardAction.IsEnabled = true;

            }
        }

        #endregion

        #region Hien thi MessageBox

        private void OpenMessageBox(string title, string message)
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox();

            // messageBox.ButtonLeftName = "Hello World";
            messageBox.ButtonRightName = "Close";

            // messageBox.ButtonLeftClick += MessageBox_LeftButtonClick;
            messageBox.ButtonRightClick += MessageBox_RightButtonClick;

            messageBox.Show(title, message);
        }

        private void MessageBox_RightButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as Wpf.Ui.Controls.MessageBox)?.Close();
        }

        #endregion

        #region Hien thi thong bao snackbar

        private void ShowSnackbar(string primaryMessage, string secondaryMessage)
        {
            (Application.Current.MainWindow as MainWindow)?.RootSnackbar.Show(primaryMessage, secondaryMessage);
        }


        #endregion
    }
}