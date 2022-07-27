using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LanCopyFiles.Events;
using LanCopyFiles.Models;
using LanCopyFiles.Services.IPAddressManager;
using LanCopyFiles.Services.SendReceiveServices;
using LanCopyFiles.Services.StorageServices;
using LanCopyFiles.Services.StorageServices.FilePrepare;
using LanCopyFiles.TransferFilesEngine.Server;
using log4net;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using XamlAnimatedGif;

namespace LanCopyFiles.Pages.ViewModels;

public class ReceiveFileBoardViewModel : BindableBase
{
    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IFileReceivingService _receivingService;
    private readonly IAppStorage _appStorage;

    private readonly IEventAggregator _eventAggregator;

    public ReceiveFileBoardViewModel(IEventAggregator eventAggregator, IFileReceivingService receivingService,
        IAppStorage appStorage)
    {
        _eventAggregator = eventAggregator;

        InitCommands();

        // var controller = AnimationBehavior.GetAnimator(dataTransferingGifImage);
        // controller.Pause();

        receivingFileAnimationStackPanel.Visibility = Visibility.Collapsed;
        showAllIPAddressesStackPanel.Visibility = Visibility.Visible;

        InitIPAddressValues();

        _receivingService = receivingService;
        _appStorage = appStorage;

        _receivingService.DataStartReceivingOnServer += OnDataStartReceiving;

        _receivingService.DataFinishReceivingOnServer += OnDataFinishReceiving;
    }

    public void AddNewReceiveLog(SendReceiveLogItem logItem)
    {
        _eventAggregator.GetEvent<SendReceiveLogAddEvent>().Publish(new List<SendReceiveLogItem>()
        {
            logItem
        });
    }

    private string _preferredIPAddress;

    public string PreferredIPAddress
    {
        get => _preferredIPAddress;
        set => SetProperty(ref _preferredIPAddress, value);
    }

    private string _allIPAddressOnAllAdapterDisplay;

    public string AllIPAddressOnAllAdapterDisplay
    {
        get => _allIPAddressOnAllAdapterDisplay;
        set => SetProperty(ref _allIPAddressOnAllAdapterDisplay, value);
    }


    private void InitIPAddressValues()
    {
        // Toan bo dia chi IP dang co tren may
        var allIPAddressesOnAllAdapters = GetIPAddressOnAllAdapters.GetAllIPv4();

        // Dia chi IP uu tien nhat
        var currentConnectionIPAddress = GetCurrentConnectionIPAddress.GetIPv4();
        // Kiem tra xem co gia tri nao tra ve hay khong
        if (!IPAddressValidator.CheckIfValidFormatIPv4Only(currentConnectionIPAddress))
        {
            // Neu khong co gia tri nao tra ve thi lay dia chi IP dau tien trong danh sach cac dia chi IP hien dang co tren may
            if (allIPAddressesOnAllAdapters.Any())
            {
                currentConnectionIPAddress = allIPAddressesOnAllAdapters.First();
            }
        }

        // preferredIPAddressTextBlock.Text =
        //     $"To receive files and folders, use this IP address: {currentConnectionIPAddress}";
        PreferredIPAddress = currentConnectionIPAddress;


        // Toan bo dia chi IP khac
        var allOTherIPAddressesOnAllAdapters =
            allIPAddressesOnAllAdapters.Except(new[] { currentConnectionIPAddress });
        // allIPAddressOnAllAdaperDisplayTextBlock.Text =
        //     "Other IP addresses on this PC:\n" + string.Join("\n", allOTherIPAddressesOnAllAdapters.ToArray());

        AllIPAddressOnAllAdapterDisplay = "Other IP addresses on this PC:\n" +
                                          string.Join("\n", allOTherIPAddressesOnAllAdapters.ToArray());
    }

    private void InitCommands()
    {
        CopyPreferredIPAddressCommand = new DelegateCommand(CopyPreferredIPAddress);
    }

    public ICommand CopyPreferredIPAddressCommand { get; private set; }

    private void CopyPreferredIPAddress()
    {
        // var currentConnectionIPAddress = preferredIPAddressTextBlock.Text.Split(':')[1].Trim();
        var currentConnectionIPAddress = PreferredIPAddress;

        Clipboard.SetText(currentConnectionIPAddress);
    }

    private void OnDataStartReceiving(object? sender, TFEServerReceivingArgs args)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            receivingFileAnimationStackPanel.Visibility = Visibility.Visible;
            showAllIPAddressesStackPanel.Visibility = Visibility.Collapsed;

            var controller = AnimationBehavior.GetAnimator(dataTransferingGifImage);
            controller.Play();
        });
    }

    private async void OnDataFinishReceiving(object? sender, TFEServerReceivingArgs args)
    {
        var receivingFileName = args.FileName;

        try
        {
            await Task.Run(() => MoveReceivedThingToDesktop(receivingFileName));

            // Them receive log
            ((ReceiveFileBoardViewModel)this.DataContext).AddNewReceiveLog(new SendReceiveLogItem()
            {
                Direction = SendReceiveDirection.Receive,
                ThingName = receivingFileName,
                WithIPAddress = args.FromIPAddress
            });

            // Received file/folder successfully
            ShowSnackbar("Just receive a file/folder from another PC!",
                $"The file/folder named {receivingFileName} was arrived, you can check it on Desktop");
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            OpenMessageBox("An error has happened",
                "There was an error when receiving a file from another PC: " + ex.Message);
        }
        finally
        {
            // Xoa file da nhan trong thu muc temp
            _appStorage.ReceivingTempFolder.Delete(receivingFileName);

            Application.Current.Dispatcher.Invoke(() =>
            {
                var controller = AnimationBehavior.GetAnimator(dataTransferingGifImage);
                controller.Pause();

                receivingFileAnimationStackPanel.Visibility = Visibility.Collapsed;
                showAllIPAddressesStackPanel.Visibility = Visibility.Visible;
            });
        }
    }

    private void MoveReceivedThingToDesktop(string receivedThingName)
    {
        // Neu ten file la duong dan day du thi xoa phan duong dan
        if (receivedThingName.StartsWith(TempFolderNames.ReceiveTempFolder))
        {
            receivedThingName =
                receivedThingName.Substring(TempFolderNames.ReceiveTempFolder.Length +
                                            1); // Cong them 1 la do co dau \
        }

        // var receivedThingPath = Path.Combine(TempFolderNames.ReceiveTempFolder, receivedThingName);

        ConfirmMessageBoxResult replaceResult = ConfirmMessageBoxResult.No;

        // Neu file hoac folder da nhan ton tai tren desktop
        if (_appStorage.ReceivingTempFolder.IsExistOnDesktop(receivedThingName))
        {
            // Hoi nguoi dung xem co muon thay the hay khong
            // replaceResult = OpenConfirmMessageBox("File or folder already exists",
            //     "There is a file or folder with the same name already existing on your desktop. Do you want to replace that file or folder?");
            replaceResult = OpenConfirmMessageBox("File or folder already exists",
                "Do you want to replace the existing file or folder?");
        }

        // Tien hanh di chuyen file hoac folder ra desktop
        _appStorage.ReceivingTempFolder.MoveToDesktop(receivedThingName,
            replaceResult == ConfirmMessageBoxResult.Yes);
    }

    #region Hien thi MessageBox lua chon

    private ConfirmMessageBoxResult OpenConfirmMessageBox(string title, string message)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox();
            messageBox.Width = 400;
            messageBox.Title = title;
            messageBox.Content = message;

            var messageBoxResult = ConfirmMessageBoxResult.Cancel;

            messageBox.ButtonLeftName = "Yes";
            messageBox.ButtonRightName = "No";

            messageBox.ButtonLeftClick += (sender, args) =>
            {
                messageBoxResult = ConfirmMessageBoxResult.Yes;
                messageBox.Close();
            };
            messageBox.ButtonRightClick += (sender, args) =>
            {
                messageBoxResult = ConfirmMessageBoxResult.No;
                messageBox.Close();
            };

            messageBox.ShowDialog();

            return messageBoxResult;
        });
    }

    #endregion

    #region Hien thi MessageBox

    private void OpenMessageBox(string title, string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox();

            messageBox.ShowFooter = false;


            // // messageBox.ButtonLeftName = "Hello World";
            // messageBox.ButtonLeftAppearance = ControlAppearance.Transparent;
            //
            // messageBox.ButtonRightName = "Close";
            // messageBox.ButtonRightAppearance = ControlAppearance.Primary;
            //
            // // messageBox.ButtonLeftClick += MessageBox_LeftButtonClick;
            // messageBox.ButtonRightClick += MessageBox_RightButtonClick;

            messageBox.Show(title, message);
        });
    }

    private void MessageBox_RightButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        (sender as Wpf.Ui.Controls.MessageBox)?.Close();
    }

    #endregion

    #region Hien thi thong bao snackbar

    private void ShowSnackbar(string primaryMessage, string secondaryMessage)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            (Application.Current.MainWindow as MainWindow)?.RootSnackbar.Show(primaryMessage, secondaryMessage);
        });
    }

    #endregion
}