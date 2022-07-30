namespace LanCopyFiles.Services.StorageServices.FilePrepare;

public interface IReceivingTempFolder
{
    public void Delete(string fileName);

    public void MoveToDesktop(string fileName, bool replaceIfExist);

    public bool IsExistOnDesktop(string fileName);
}