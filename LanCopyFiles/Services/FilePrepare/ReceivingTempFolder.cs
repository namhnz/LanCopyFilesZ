using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using LanCopyFiles.Services.PackingServices;

namespace LanCopyFiles.Services.FilePrepare;

public class ReceivingTempFolder
{
    public void Delete(string fileName)
    {
        var filePath = Path.Combine(TempFolderNames.ReceiveTempFolderPath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public void MoveToDesktop(string fileName) /* Vi du: test_file.zip, dark.mkv, mind-map.zip.iszipfile */
    {
        // Duong dan day du cua file trong thu muc receive temp
        var filePath = Path.Combine(TempFolderNames.ReceiveTempFolderPath, fileName);

        var fileExtension = Path.GetExtension(fileName);

        // Neu la file .zip thi giai nen ra desktop
        if (fileExtension == ".zip")
        {
            var folderName =
                fileName[0..^4]; /* 4 la do dai cua ".zip", nguon: https://stackoverflow.com/a/9469003/7182661 */

            // Duong dan thu muc sau khi giai nen ra desktop
            var destinationExtractFolderPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), folderName);

            // // Kiem tra xem folder giai nen da ton tai hay chua, neu co thi xoa folder do di
            // if (Directory.Exists(destinationExtractFolderPath))
            // {
            //     Directory.Delete(destinationExtractFolderPath);
            // }

            // Giai nen ra file zip desktop
            ZipService.Instance.ExtractZipToFolder(filePath, destinationExtractFolderPath);

            return;
        }

        // Neu la file khong phai duoi .zip thi di chuyen file ra desktop
        var destinationFileName = fileName;


        // Neu file co duoi .iszipfile thi bo duoi .iszipfile, neu khong co thi giu nguyen nhu o tren

        if (fileExtension == ".iszipfile")
        {
            destinationFileName = fileName[0..^10]; /* loai bo duoi .iszipfile */
        }

        var destinationFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            destinationFileName);

        // Di chuyen file ra desktop
        File.Move(filePath, destinationFilePath);
    }

    public bool IsExistOnDesktop(string fileName)
    {
        // Duong dan day du cua file trong thu muc receive temp
        var filePath = Path.Combine(TempFolderNames.ReceiveTempFolderPath, fileName);

        var fileExtension = Path.GetExtension(fileName);

        // Neu la file .zip thi do la folder
        if (fileExtension == ".zip")
        {
            var folderName =
                fileName[0..^4]; /* 4 la do dai cua ".zip", nguon: https://stackoverflow.com/a/9469003/7182661 */

            // Duong dan thu muc neu giai nen ra desktop
            var destinationExtractFolderPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), folderName);

            // Kiem tra xem folder da ton tai tren desktop hay chua
            return Directory.Exists(destinationExtractFolderPath);
        }

        // Neu la file khong phai duoi .zip thi la mot file binh thuong
        var destinationFileName = fileName;

        // Neu file co duoi .iszipfile thi bo duoi .iszipfile, neu khong co thi giu nguyen nhu o tren

        if (fileExtension == ".iszipfile")
        {
            destinationFileName = fileName[0..^10]; /* loai bo duoi .iszipfile */
        }

        var destinationFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            destinationFileName);

        // Di chuyen file ra desktop
        return Directory.Exists(destinationFilePath);
    }


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

    // public static bool IsFolderAlreadyExistOnDesktop(string sourceZipFileName)
    // {
    //     // Giai nen file zip trong thu muc temp vao thu muc dich
    //
    //     // Lay ten file zip nguon
    //     // Kiem tra ten file zip da co duoi .zip chua
    //     if (!sourceZipFileName.EndsWith(".zip"))
    //     {
    //         sourceZipFileName += ".zip";
    //     }
    //
    //     var sourceZipFileNameWithoutExtension =
    //         sourceZipFileName[0..^4]; /* 4 la do dai cua ".zip", nguon: https://stackoverflow.com/a/9469003/7182661 */
    //
    //     // Thu muc dich giai nen full path nam o desktop
    //
    //     var destinationExtractFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
    //         sourceZipFileNameWithoutExtension);
    //
    //     // Kiem tra xem file zip da ton tai hay chua, neu co thi tra ve true
    //     if (Directory.Exists(destinationExtractFolderPath))
    //     {
    //         return true;
    //     }
    //
    //     // Con lai tra ve false
    //     return false;
    // }

    public static void ExtractFolderCopied(string sourceZipFileName)
    {
        // // Lay thu muc dang chay app
        // var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;
        //
        // // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        // var copyTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.ReceiveTempFolder);
        //
        // if (!Directory.Exists(copyTempFolderPath))
        // {
        //     Directory.CreateDirectory(copyTempFolderPath);
        // }

        // Giai nen file zip trong thu muc temp vao thu muc dich

        // Lay ten file zip nguon
        // Kiem tra ten file zip da co duoi .zip chua
        // if (!sourceZipFileName.EndsWith(".zip"))
        // {
        //     sourceZipFileName += ".zip";
        // }

        // var sourceZipFileNameWithoutExtension = sourceZipFileName[0..^4]; /* 4 la do dai cua ".zip", nguon: https://stackoverflow.com/a/9469003/7182661 */

        // // Lay file path cua file zip
        // var sourceZipFilePath = Path.Combine(copyTempFolderPath, sourceZipFileName);
        //
        // // Thu muc dich giai nen full path nam o desktop
        //
        // var destinationExtractFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        //     sourceZipFileNameWithoutExtension);

        // // Kiem tra xem file zip da ton tai hay chua, neu co thi xoa file zip do di
        // if (Directory.Exists(destinationExtractFolderPath))
        // {
        //     Directory.Delete(destinationExtractFolderPath);
        // }

        // Tao file nen vao trong thu muc temp
        // ExtractZipToFolder(sourceZipFilePath, destinationExtractFolderPath);
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