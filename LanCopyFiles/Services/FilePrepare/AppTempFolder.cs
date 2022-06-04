using System;
using System.IO;

namespace LanCopyFiles.Services.FilePrepare;

public class AppTempFolder
{
    public static string[] GetAllFilePathsInTempFolder()
    {
        // Lay thu muc dang chay app
        var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;

        // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        var copyTempFolderPath = Path.Combine(currentAppFolder, "temp");

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
        var copyTempFolderPath = Path.Combine(currentAppFolder, "temp");

        if (Directory.Exists(copyTempFolderPath))
        {
            
            // Xoa toan bo moi thu ben trong temp folder
            // Nguon: https://stackoverflow.com/a/1288747/7182661

            DirectoryInfo di = new DirectoryInfo(copyTempFolderPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}