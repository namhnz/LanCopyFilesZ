using System.IO;
using System.Linq;

namespace LanCopyFiles.Services.FileSystemAnalyze;

public class FileSystemStructureBuilder
{
    public static FileSystemNode GetStructure(string fileSystemFullPath)
    {
        // get the file attributes for file or directory
        FileAttributes attr = File.GetAttributes(fileSystemFullPath);

        if (attr.HasFlag(FileAttributes.Directory))
        {
            // Its a directory

            // Lay chi ten thu muc can tao cau truc: https://stackoverflow.com/a/5229311/7182661
            var rootFolderNameOnly = new DirectoryInfo(fileSystemFullPath).Name;

            // Tao node la thu muc can tao cau truc
            var fileSystemNode = new FileSystemNode();
            fileSystemNode.Name = rootFolderNameOnly;
            fileSystemNode.Type = FileSystemType.Folder;

            // Lay danh sach cac folder va file trong thu muc: https://stackoverflow.com/a/23570998/7182661
            string[] allFileSystemDirectInsideEntries = Directory.GetFileSystemEntries(fileSystemFullPath, "*", SearchOption.TopDirectoryOnly);
            fileSystemNode.Children = allFileSystemDirectInsideEntries.Select(GetStructure).ToList();

            return fileSystemNode;
        }
        else
        {
            // Its a file
            var fileNameOnly = Path.GetFileName(fileSystemFullPath);
            var fileSystemNode = new FileSystemNode();
            fileSystemNode.Name = fileNameOnly;
            fileSystemNode.Type = FileSystemType.File;

            return fileSystemNode;
        }
        
    }

    public static string GetStructureToJson(string fileSystemFullPath)
    {
        var fileSystemNode = GetStructure(fileSystemFullPath);
        string fileSystemNodeJson = Newtonsoft.Json.JsonConvert.SerializeObject(fileSystemNode);
        return fileSystemNodeJson;
    }
}