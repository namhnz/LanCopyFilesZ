using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LanCopyFiles.Configs;
using LanCopyFiles.Extensions;
using LanCopyFiles.Models;
using LanCopyFiles.Services.IPAddressManager;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;
using log4net;
using Ookii.Dialogs.Wpf;
using Wpf.Ui.Controls;

namespace LanCopyFiles.Pages.Views
{
    /// <summary>
    /// Interaction logic for SendFilesBoard.xaml
    /// </summary>
    public partial class SendFilesBoard : UiPage
    {
        

        public SendFilesBoard()
        {
            InitializeComponent();

            

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