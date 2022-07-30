using LanCopyFiles.Services.StorageServices.FilePrepare;

namespace LanCopyFiles.Services.StorageServices;

public interface IAppStorage
{
    public IReceivingTempFolder ReceivingTempFolder { get; }
    public ISendingTempFolder SendingTempFolder { get; }

    public void ClearTempFolders();

    public void EnsureTempFoldersExist();
}