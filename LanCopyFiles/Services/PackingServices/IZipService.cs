namespace LanCopyFiles.Services.PackingServices;

public interface IZipService
{
    public void CompressFolderToZip(string sourceFolderPath, string destinationZipFilePath);

    public void ExtractZipToFolder(string sourceZipFilePath, string destinationFolder)
}