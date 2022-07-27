using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using LanCopyFiles.Configs;
using LanCopyFiles.Models;
using LanCopyFiles.Services.IPAddressManager;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;
using log4net;
using Prism.Mvvm;

namespace LanCopyFiles.Pages.ViewModels;

public class SendFileBoardViewModel : BindableBase
{
    private readonly IAppStorage _appStorage;

    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public SendFileBoardViewModel(IAppStorage appStorage)
    {
        _appStorage = appStorage;
        LoadConfigs();
    }

    #region Luu lai cac cai dat
    
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
        GlobalAppConfigs.Instance.SendFilesConfigs
            .SetSavedDestinationPCIPAddressConfigValue(destinationPCIPAddress);

        // Hien thi so luong file va folder se copy
        SendingStatusTextBlock.Text = $"There will be {thingPaths.Length} file(s) and folder(s) sent";

        // Tao trinh gui file moi
        var fileSendingService = new FileSendingService(thingPaths, destinationPCIPAddress);

        // Hien thi tien trinh gui file len UI
        fileSendingService.FilesSendingProgressChanged += OnSendingProgressChanged;

        // Gui file den may dich
        var sendFileResults = await fileSendingService.SendFilesToDestinationPC();

        // Tam dung 2s de progress bar chay len duoc 100%
        await Task.Delay(TimeSpan.FromSeconds(2));
        ClearSendingProgressStatus();

        return sendFileResults;
    }

    private void ClearSendingProgressStatus()
    {
        SendingProgressBar.SetPercent(0);
        SendingStatusTextBlock.Text = string.Empty;
    }

    private void OnSendingProgressChanged(object? sender, FilesSendingProgressInfoArgs args)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            SendingProgressBar.SetPercent(args.TotalSendingPercentage);
            SendingStatusTextBlock.Text =
                $"Transferring file {args.SendingFileName}: {Math.Ceiling(args.TotalSendingPercentage)}%";
        });

        Trace.WriteLine($"Dang gui file :{args.TotalSendingPercentage}%");
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
                _appStorage.SendingTempFolder.AddMany(thingPaths);
                Trace.WriteLine("Da copy xong cac file vao thu muc temp");
            });

            SendingProgressBar.IsIndeterminate = false;
            SendingStatusTextBlock.Text = "The file(s)/folder(s) is/are ready for transfer";

            // Lay duong dan tat cac cac file da copy vao trong thu muc send temp
            var allFilesInTempFolder = AppStorage.Instance.SendingTempFolder.GetAll().ToArray();

            // Gui file den may dich
            var sendingResults = await SendFilesToDestinationPC(allFilesInTempFolder, destinationPCIPAddress);

            // Dem so luong file va folder da gui thanh cong
            var thingsSendSuccessCount = sendingResults.Count(x => x);

            // Hien thi ket qua
            ShowSnackbar("All the work has been completed!",
                $"There are {thingsSendSuccessCount} file(s) or folder(s) that were successfully sent and {thingPaths.Length - thingsSendSuccessCount} that were not");
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            ClearSendingProgressStatus();
            OpenMessageBox("Sending failed", "An error has happened: " + ex.Message);
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

}