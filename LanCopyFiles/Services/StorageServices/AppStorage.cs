using System;
using System.IO;
using System.Threading;
using LanCopyFiles.Services.StorageServices.FilePrepare;

namespace LanCopyFiles.Services.StorageServices;

public class AppStorage: IAppStorage
{
    
    public ISendingTempFolder SendingTempFolder { get; private set; }

    public IReceivingTempFolder ReceivingTempFolder { get; private set; }

    public AppStorage(ISendingTempFolder sendingTempFolder, IReceivingTempFolder receivingTempFolder)
    {
        SendingTempFolder = sendingTempFolder;
        ReceivingTempFolder = receivingTempFolder;
        EnsureTempFoldersExist();
    }

    public void ClearTempFolders()
    {
        // // Lay thu muc dang chay app
        // var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;
        //
        // // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        // var sendTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.SendTempFolder);
        var sendTempFolderPath = TempFolderNames.SendTempFolderPath;

        if (Directory.Exists(sendTempFolderPath))
        {
            // Xoa toan bo moi thu ben trong temp folder
            // Nguon: https://stackoverflow.com/a/1288747/7182661

            DirectoryInfo di = new DirectoryInfo(sendTempFolderPath);

            foreach (FileInfo file in
                     di.GetFiles("*", SearchOption.AllDirectories)) //https://stackoverflow.com/a/14305610/7182661
            {
                while (FileLockChecker.IsFileLocked(file))
                    Thread.Sleep(1000);
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        // // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        // var receiveTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.ReceiveTempFolder);

        var receiveTempFolderPath = TempFolderNames.ReceiveTempFolderPath;

        if (Directory.Exists(receiveTempFolderPath))
        {
            // Xoa toan bo moi thu ben trong temp folder
            // Nguon: https://stackoverflow.com/a/1288747/7182661

            DirectoryInfo di = new DirectoryInfo(receiveTempFolderPath);

            foreach (FileInfo file in
                     di.GetFiles("*", SearchOption.AllDirectories)) //https://stackoverflow.com/a/14305610/7182661
            {
                while (FileLockChecker.IsFileLocked(file))
                    Thread.Sleep(1000);
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }

    public void EnsureTempFoldersExist()
    {
        // Lay thu muc dang chay app
        var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;

        // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        var receiveTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.ReceiveTempFolder);

        if (!Directory.Exists(receiveTempFolderPath)) Directory.CreateDirectory(receiveTempFolderPath);

        // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        var sendTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.SendTempFolder);

        if (!Directory.Exists(sendTempFolderPath)) Directory.CreateDirectory(sendTempFolderPath);
    }
}