using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace LanCopyFiles.Services.FilePrepare;

public class ReceivingTempFolder
{
    public static void EnsureReceiveTempFolderExist()
    {
        // // Lay thu muc dang chay app
        // var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;

        // // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        // var copyTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.ReceiveTempFolder);
        //
        // if (!Directory.Exists(copyTempFolderPath))
        // {
        //     Directory.CreateDirectory(copyTempFolderPath);
        // }
    }

    public static bool IsFolderAlreadyExistOnDesktop(string sourceZipFileName)
    {
        // Giai nen file zip trong thu muc temp vao thu muc dich

        // Lay ten file zip nguon
        // Kiem tra ten file zip da co duoi .zip chua
        if (!sourceZipFileName.EndsWith(".zip"))
        {
            sourceZipFileName += ".zip";
        }

        var sourceZipFileNameWithoutExtension = sourceZipFileName[0..^4]; /* 4 la do dai cua ".zip", nguon: https://stackoverflow.com/a/9469003/7182661 */

        // Thu muc dich giai nen full path nam o desktop

        var destinationExtractFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            sourceZipFileNameWithoutExtension);

        // Kiem tra xem file zip da ton tai hay chua, neu co thi tra ve true
        if (Directory.Exists(destinationExtractFolderPath))
        {
            return true;
        }

        // Con lai tra ve false
        return false;
    }

    public static void ExtractFolderCopied(string sourceZipFileName)
    {
        // Lay thu muc dang chay app
        var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;

        // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        var copyTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.ReceiveTempFolder);

        if (!Directory.Exists(copyTempFolderPath))
        {
            Directory.CreateDirectory(copyTempFolderPath);
        }

        // Giai nen file zip trong thu muc temp vao thu muc dich

        // Lay ten file zip nguon
        // Kiem tra ten file zip da co duoi .zip chua
        if (!sourceZipFileName.EndsWith(".zip"))
        {
            sourceZipFileName += ".zip";
        }

        var sourceZipFileNameWithoutExtension = sourceZipFileName[0..^4]; /* 4 la do dai cua ".zip", nguon: https://stackoverflow.com/a/9469003/7182661 */

        // Lay file path cua file zip
        var sourceZipFilePath = Path.Combine(copyTempFolderPath, sourceZipFileName);

        // Thu muc dich giai nen full path nam o desktop

        var destinationExtractFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            sourceZipFileNameWithoutExtension);

        // Kiem tra xem file zip da ton tai hay chua, neu co thi xoa file zip do di
        if (Directory.Exists(destinationExtractFolderPath))
        {
            Directory.Delete(destinationExtractFolderPath);
        }

        // Tao file nen vao trong thu muc temp
        ExtractZipToFolder(sourceZipFilePath, destinationExtractFolderPath);
    }

    // private static void ExtractZipToFolder(string sourceZipFilePath, string destinationFolder)
    // {
    //     if (!File.Exists(sourceZipFilePath))
    //     {
    //         throw new FileNotFoundException("Khong tim  thay file zip can giai nen");
    //     }
    //
    //     FastZip fastZip = new FastZip();
    //     string fileFilter = null;
    //
    //     // Will always overwrite if target filenames already exist
    //     fastZip.ExtractZip(sourceZipFilePath, destinationFolder, fileFilter);
    // }
}