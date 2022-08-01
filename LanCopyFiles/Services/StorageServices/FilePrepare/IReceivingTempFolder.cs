namespace LanCopyFiles.Services.StorageServices.FilePrepare;

public interface IReceivingTempFolder
{
    public void Delete(string fileName);

    public void MoveToDesktop(string fileName, bool overwriteIfExist);

    public bool IsExistOnDesktop(string fileName);
}