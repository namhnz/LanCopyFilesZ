using LanCopyFiles.Services.StorageServices.FilePrepare;

namespace LanCopyFiles.Services.StorageServices;

public interface IAppStorage
{
    public ReceivingTempFolder ReceivingTempFolder { get; }
    public SendingTempFolder SendingTempFolder { get; }

    public void ClearTempFolders();

    public void EnsureTempFoldersExist();
}