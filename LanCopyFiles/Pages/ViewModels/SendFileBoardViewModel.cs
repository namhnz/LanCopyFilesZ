using LanCopyFiles.Configs;
using log4net;
using Prism.Mvvm;

namespace LanCopyFiles.Pages.ViewModels;

public class SendFileBoardViewModel : BindableBase
{
    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public SendFileBoardViewModel()
    {
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

}