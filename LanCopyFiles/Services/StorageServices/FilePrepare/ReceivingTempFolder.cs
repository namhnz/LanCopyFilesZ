using System;
using System.IO;
using LanCopyFiles.Services.PackingServices;

namespace LanCopyFiles.Services.StorageServices.FilePrepare;

public class ReceivingTempFolder: IReceivingTempFolder
{
    private readonly IZipService _zipService;

    public ReceivingTempFolder(IZipService zipService)
    {
        _zipService = zipService;
    }

    public void Delete(string fileName)
    {
        var filePath = Path.Combine(TempFolderNames.ReceiveTempFolderPath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public void
        MoveToDesktop(string fileName, bool overwriteIfExist) /* Vi du: test_file.zip, dark.mkv, mind-map.zip.iszipfile */
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

            // Kiem tra xem folder giai nen da ton tai hay chua, neu co thi xoa folder do di
            if (overwriteIfExist && Directory.Exists(destinationExtractFolderPath))
            {
                Directory.Delete(destinationExtractFolderPath);
            }

            // Giai nen ra file zip desktop
            if (!Directory.Exists(destinationExtractFolderPath))
            {
                _zipService.ExtractZipToFolder(filePath, destinationExtractFolderPath);
            }

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

        // // Kiem tra xem file da ton tai tren desktop hay chua, neu co thi xoa file do tren desktop di de thay the file moi
        // if (overwriteIfExist && File.Exists(destinationFilePath))
        // {
        //     File.Delete(destinationFilePath);
        // }
        //
        // // Di chuyen file ra desktop
        // if (!File.Exists(destinationFilePath))
        // {
        //     File.Move(filePath, destinationFilePath);
        // }

        // // Kiem tra xem file da ton tai tren desktop hay chua, neu co thi ghi de file tren desktop bang file moi

        File.Move(filePath, destinationFilePath, overwriteIfExist);
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

        // Kiem tra xem file da ton tai tren desktop hay chua
        return File.Exists(destinationFilePath);
    }
}