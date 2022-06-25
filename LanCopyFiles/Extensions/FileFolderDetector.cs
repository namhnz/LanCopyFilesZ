using System.IO;

namespace LanCopyFiles.Extensions;

public class FileFolderDetector
{
    public static bool IsPathFolder(string path)
    {
        // Kiem tra xem duong dan la file hay folder: https://stackoverflow.com/a/1395226/7182661
        // get the file attributes for file or directory
        FileAttributes attr = File.GetAttributes(path);
        
        if (attr.HasFlag(FileAttributes.Directory))
        {
            // Its a directory
            return true;
            
        }
        else
        {
            // Its a file
            return false;
        }
    }
}