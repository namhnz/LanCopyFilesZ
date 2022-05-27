using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Threading;
using EasyFileTransfer;
using LanCopyFiles.Extensions;
using LanCopyFiles.Services;
using Ookii.Dialogs.Wpf;
using SharpConfig;

namespace LanCopyFiles.Pages
{
    /// <summary>
    /// Interaction logic for SendFilesBoard.xaml
    /// </summary>
    public partial class SendFilesBoard : Page
    {
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

            InitUpdateProgressBarTimer();
        }

        // Nguon: https://stackoverflow.com/a/5663329/7182661
        private void FilesPickerContainerStackPanel_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                // HandleFileOpen(files[0]);
                MessageBox.Show(files[0]);
            }
        }

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

            var filesSelectDialog = new VistaOpenFileDialog();
            filesSelectDialog.Multiselect = true;

            var selectResult = filesSelectDialog.ShowDialog();
            
            if (selectResult ?? false)
            {
                // string path = filesSelectDialog.FileName;
                // MessageBox.Show(path);
                var filePaths = filesSelectDialog.FileNames;

                try
                {
                    var copyResults = await SendFilesToServer(filePaths, this.serverIPTextBox.Text,
                        int.Parse(serverPortTextBox.Text));
                    var successCopiedFilesCount = copyResults.Count(x => x);

                    MessageBox.Show(
                        $"Copied {successCopiedFilesCount} file(s) successfully and {copyResults.Count - successCopiedFilesCount} fail");
                }
                catch (Exception ex)
                {
                    ResetCopyingStatusText();
                    MessageBox.Show("An error has happened: " + ex.Message);

                    Trace.WriteLine(ex);
                }
            }
        }

        #endregion

        private void FilesPickerCardAction_OnClick(object sender, RoutedEventArgs e)
        {
            _filesOrFoldersPickerContainerClickWaitTimer.Start();
        }

        private void FilesPickerCardAction_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Stop the timer from ticking.
            _filesOrFoldersPickerContainerClickWaitTimer.Stop();

            Trace.WriteLine("Double Click");
            e.Handled = true;

            var foldersSelectDialog = new VistaFolderBrowserDialog();
            var selectResult = foldersSelectDialog.ShowDialog();

            if (selectResult ?? false)
            {
                string path = foldersSelectDialog.SelectedPath;
                MessageBox.Show(path);
            }
        }
        
        private void FilesOrFoldersPickerContextMenuPasteItem_OnClick(object sender, RoutedEventArgs e)
        {
            // Stop the timer from ticking.
            _filesOrFoldersPickerContainerClickWaitTimer.Stop();

            Trace.WriteLine("Double Click");
            e.Handled = true;

            var foldersSelectDialog = new VistaFolderBrowserDialog();
            var selectResult = foldersSelectDialog.ShowDialog();

            if (selectResult ?? false)
            {
                string path = foldersSelectDialog.SelectedPath;



                // MessageBox.Show(path);
            }
        }

        private async Task<List<bool>> SendFilesToServer(string[] filePaths, string serverIP, int serverPort)
        {
            // Hien thi thong bao trang thai dang ket noi den dia chi IP
            SetCopyingStatusText($"Connecting to remote IP address: {serverIP}:{serverPort}");

            if (!IPAddressValidation.ValidateIPv4(serverIP) || serverPort <= 0)
            {
                throw new ArgumentException("Invalid destination IP address or port");
            }
            
            // Kiem tra xem co ket noi duoc den server khong
            await Task.Run(() =>
            {
                if (!LanConnection.PingHost(serverIP, serverPort))
                {
                    throw new WebException($"Can't connect to {serverIP}:{serverPort}");
                }
            });

            // Hien thi thong bao trang thai ket noi thanh cong
            SetCopyingStatusText($"Connect to remote IP address: {serverIP}:{serverPort} successfully");

            // Luu lai dia chi IP trong truong hop kiem tra ket noi thanh cong
            _ipCopyToSectionConfig["IPAddress"].StringValue = serverIP;
            _ipCopyToSectionConfig["Port"].IntValue = serverPort;
            SaveConfigs();

            // Hien thi so luong file va folder se copy
            SetCopyingStatusText($"{filePaths.Length} file(s) and folder(s) will be copied");

            _updateProgressBarTimer.Start();
            _countTotalFilesAndFoldersCopying = filePaths.Length;
            
            var copyResultTask = await Task.Run(() =>
            {

                var sendFileResults = new List<bool>();

                for (int i = 0; i < filePaths.Length; i++)
                {
                    _fileOrFolderCopyingIndex = i;

                    if (File.Exists(filePaths[i]))
                    {
                        // Dung de hien thi len thong bao trang thai
                        _copyingFileOrFolderName = Path.GetFileName(filePaths[i]);

                        var response = EftClient.Send(filePaths[i], serverIP, serverPort);
                        if (response != null && response.status == 1)
                        {
                            sendFileResults.Add(true);
                        }
                        else
                        {
                            sendFileResults.Add(false);
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException($"File {filePaths[i]} doesn't exist");
                    }
                }

                // _updateProgressBarTimer.Stop();

                return sendFileResults;
            });

            ResetCopyingProgress();
            ResetCopyingStatusText();

            return copyResultTask;
        }

        private void ResetCopyingProgress()
        {
            _fileOrFolderCopyingIndex = 0;
            _countTotalFilesAndFoldersCopying = 0;

            SetCopyingProgress();
            _updateProgressBarTimer.Stop();
        }

        private void SetCopyingProgress()
        {
            if (_countTotalFilesAndFoldersCopying > 0)
            {
                // CopyingProgressBar.Value = EftClient.ProgressValue * (_fileOrFolderCopyingIndex + 1) /
                //                           (double)_countTotalFilesAndFoldersCopying;

                CopyingProgressBar.SetPercent(EftClient.ProgressValue * 1 /
                                              (double)_countTotalFilesAndFoldersCopying +
                                              100 * _fileOrFolderCopyingIndex / (double)_countTotalFilesAndFoldersCopying);

                Trace.WriteLine($"Dang copy: {CopyingProgressBar.Value}%");

                // Hien thi len thong bao trang thai
                SetCopyingStatusText($"Copying file {_copyingFileOrFolderName}: {Math.Ceiling(CopyingProgressBar.Value)}%");
            }
            else
            {
                CopyingProgressBar.SetPercent(0);
            }
        }

        private int _fileOrFolderCopyingIndex;
        private int _countTotalFilesAndFoldersCopying;

        // Nguon: https://stackoverflow.com/a/11560151/7182661
        private DispatcherTimer _updateProgressBarTimer;

        private void InitUpdateProgressBarTimer()
        {
            _updateProgressBarTimer = new DispatcherTimer();
            _updateProgressBarTimer.Tick += UpdateProgressBarTimer_Tick;
            _updateProgressBarTimer.Interval = new TimeSpan(0, 0, 0,1);
            // _updateProgressBarTimer.Start();
        }

        private void UpdateProgressBarTimer_Tick(object sender, EventArgs e)
        {
            // code goes here
            SetCopyingProgress();
        }

        #region Hien thi thong tin ve tac vu dang thuc hien

        private string _copyingFileOrFolderName;

        private void SetCopyingStatusText(string copyingStatusText)
        {
            CopyingStatusTextBox.Text = copyingStatusText;
        }

        private void ResetCopyingStatusText()
        {
            _copyingFileOrFolderName = "";
            CopyingStatusTextBox.Text = "";
        }

        #endregion

        #region Luu lai cac cai dat

        private Section _ipCopyToSectionConfig;
        private Configuration _config;


        private void LoadConfigs()
        {
            if (File.Exists("user_data.cfg"))
            {
                _config = Configuration.LoadFromFile("user_data.cfg");
            }
            else
            {
                _config = new Configuration();
            }

            _ipCopyToSectionConfig = _config["IPCopyTo"];

            // Lay cac gia tri cac duong dan da luu
            var ipCopyToAddress = _ipCopyToSectionConfig["IPAddress"].StringValue;
            if (!string.IsNullOrEmpty(ipCopyToAddress))
            {
                serverIPTextBox.Text = ipCopyToAddress;
            }

            var ipCopyToPort = _ipCopyToSectionConfig["Port"].IntValue;
            if (ipCopyToPort > 0 && ipCopyToPort <= 65535)
            {
                serverPortTextBox.Text = ipCopyToPort.ToString();
            }
            
        }

        private void SaveConfigs()
        {
            _config.SaveToFile("user_data.cfg");
        }

        #endregion
    }
}
