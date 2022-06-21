using System;
using System.IO;
using System.Threading;

namespace LanCopyFiles.Services.FilePrepare;

public class AppTempFolder
{
    public static string[] GetAllFilePathsInSendTempFolder()
    {
        // Lay thu muc dang chay app
        var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;

        // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        var copyTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.SendTempFolder);

        if (!Directory.Exists(copyTempFolderPath))
        {
            return Array.Empty<string>();
        }

        // Lay toan bo file trong thu muc temp
        string[] allFiles = Directory.GetFiles(copyTempFolderPath, "*.*", SearchOption.TopDirectoryOnly);
        
        return allFiles;
    }

    public static void DeleteAllFilesInTempFolder()
    {

        // Lay thu muc dang chay app
        var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;

        // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        var sendTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.SendTempFolder);

        if (Directory.Exists(sendTempFolderPath))
        {
            
            // Xoa toan bo moi thu ben trong temp folder
            // Nguon: https://stackoverflow.com/a/1288747/7182661

            DirectoryInfo di = new DirectoryInfo(sendTempFolderPath);

            foreach (FileInfo file in di.GetFiles("*", SearchOption.AllDirectories)) //https://stackoverflow.com/a/14305610/7182661
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

        // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        var receiveTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.ReceiveTempFolder);

        if (Directory.Exists(receiveTempFolderPath))
        {

            // Xoa toan bo moi thu ben trong temp folder
            // Nguon: https://stackoverflow.com/a/1288747/7182661

            DirectoryInfo di = new DirectoryInfo(receiveTempFolderPath);

            foreach (FileInfo file in di.GetFiles("*", SearchOption.AllDirectories)) //https://stackoverflow.com/a/14305610/7182661
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
}