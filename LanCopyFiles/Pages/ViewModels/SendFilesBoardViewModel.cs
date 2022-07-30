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
using Wpf.Ui.Mvvm.Contracts;

namespace LanCopyFiles.Pages.ViewModels;

public class SendFilesBoardViewModel : BindableBase
{
    private readonly IAppStorage _appStorage;
    private readonly IGlobalAppConfigs _globalAppConfigs;
    private readonly ISnackbarService _snackbarService;

    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public SendFilesBoardViewModel(IAppStorage appStorage, IGlobalAppConfigs globalAppConfigs, ISnackbarService snackbarService)
    {
        _appStorage = appStorage;
        _globalAppConfigs = globalAppConfigs;
        _snackbarService = snackbarService;
        LoadConfigs();

        IsFilesPickerAllowChoosing = true;
    }

    #region Luu lai cac cai dat
    
    private void LoadConfigs()
    {
        var destinationPCIPAddressSaved =
            _globalAppConfigs.SendFilesConfigs.GetSavedDestinationPCIPAddressConfigValue();

        if (!string.IsNullOrEmpty(destinationPCIPAddressSaved))
        {
            DestinationPCIPAddress = destinationPCIPAddressSaved;
        }
    }

    #endregion

    private string _destinationPCIPAddress;

    public string DestinationPCIPAddress
    {
        get => _destinationPCIPAddress;
        set => SetProperty(ref _destinationPCIPAddress, value);
    }



    private string _sendingStatusText;

    public string SendingStatusText
    {
        get => _sendingStatusText;
        set => SetProperty(ref _sendingStatusText, value);
    }

    private double _sendingProgressValue;

    public double SendingProgressValue
    {
        get => _sendingProgressValue;
        set => SetProperty(ref _sendingProgressValue, value);
    }

    private bool _isSendingProgressBarIndeterminate;

    public bool IsSendingProgressBarIndeterminate
    {
        get => _isSendingProgressBarIndeterminate;
        set => SetProperty(ref _isSendingProgressBarIndeterminate, value);
    }

    private bool _isFilesPickerAllowChoosing;

    public bool IsFilesPickerAllowChoosing
    {
        get => _isFilesPickerAllowChoosing;
        set => SetProperty(ref _isFilesPickerAllowChoosing, value);
    }




    #region Gui cac file den server

    private async Task<List<bool>> SendFilesToDestinationPC(string[] thingPaths, string destinationPCIPAddress)
    {
        // Hien thi thong bao trang thai dang ket noi den dia chi IP
        SendingStatusText = $"Connecting to the destination PC's IP address: {destinationPCIPAddress}";

        // Kiem tra xem co ket noi duoc den server khong
        await Task.Run(() =>
        {
            if (!IPAddressValidator.TestConnectionUsingPingHost(destinationPCIPAddress))
            {
                throw new WebException($"Can't connect to {destinationPCIPAddress}");
            }
        });

        // Hien thi thong bao trang thai ket noi thanh cong
        SendingStatusText = $"Connected to the destination PC: {destinationPCIPAddress}";

        // Luu lai dia chi IP trong truong hop kiem tra ket noi thanh cong
        _globalAppConfigs.SendFilesConfigs
            .SetSavedDestinationPCIPAddressConfigValue(destinationPCIPAddress);

        // Hien thi so luong file va folder se copy
        SendingStatusText = $"There will be {thingPaths.Length} file(s) and folder(s) sent";

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
        // SendingProgressBar.SetPercent(0);
        SendingProgressValue = 0;
        SendingStatusText = string.Empty;
    }

    private void OnSendingProgressChanged(object? sender, FilesSendingProgressInfoArgs args)
    {
        // Application.Current.Dispatcher.Invoke(() =>
        // {
        //     SendingProgressBar.SetPercent(args.TotalSendingPercentage);
        //     SendingStatusText =
        //         $"Transferring file {args.SendingFileName}: {Math.Ceiling(args.TotalSendingPercentage)}%";
        // });

        SendingProgressValue = args.TotalSendingPercentage;
        SendingStatusText =
            $"Transferring file {args.SendingFileName}: {Math.Ceiling(args.TotalSendingPercentage)}%";

        Trace.WriteLine($"Dang gui file :{args.TotalSendingPercentage}%");
    }

    #endregion



    #region Copy for all

    public async Task StartSendingProcess(string[] thingPaths)
    {
        try
        {
            // Kiem tra xem thu dia chi IP may dich da day du hay chua
            // var destinationPCIPAddress = DestinationPCIPAddress;

            if (!IPAddressValidator.CheckIfValidFormatIPv4Only(DestinationPCIPAddress))
            {
                OpenMessageBox("Invalid IP address", "The destination PC's IP address is invalid");
                return;
            }

            // Disable panel truyen file trong khi dang chuyen file sang may khac
            IsFilesPickerAllowChoosing = false;

            // Chuan bi cac file can copy vao thu muc temp
            // Chinh thanh progress bar sang trang thai indetermine va status thanh prepare copying file(s)/folder(s)
            IsSendingProgressBarIndeterminate = true;
            SendingStatusText = "Getting ready to transfer file(s) or folder(s)";

            // Copy cac file va folder vao thu muc send temp
            await Task.Run(() =>
            {
                _appStorage.SendingTempFolder.AddMany(thingPaths);
                Trace.WriteLine("Da copy xong cac file vao thu muc temp");
            });

            IsSendingProgressBarIndeterminate = false;
            SendingStatusText = "The file(s)/folder(s) is/are ready for transfer";

            // Lay duong dan tat cac cac file da copy vao trong thu muc send temp
            var allFilesInTempFolder = _appStorage.SendingTempFolder.GetAll().ToArray();

            // Gui file den may dich
            var sendingResults = await SendFilesToDestinationPC(allFilesInTempFolder, DestinationPCIPAddress);

            // Dem so luong file va folder da gui thanh cong
            var thingsSendSuccessCount = sendingResults.Count(x => x);

            // Hien thi ket qua
            _snackbarService.Show("All the work has been completed!",
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
            _appStorage.ClearTempFolders();

            // Enable lai panel truyen file trong sau khi da chuyen file sang may khac
            IsFilesPickerAllowChoosing = true;
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
}