using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace LanCopyFiles.Services.PackingServices;

public class ZipService
{
    private static ZipService _instance;

    public static ZipService Instance
    {
        get { return _instance ??= new ZipService(); }
    }

    public void CompressFolderToZip(string sourceFolderPath, string destinationZipFilePath)
    {
        // Them duoi .zip vao file neu chua co
        if (!destinationZipFilePath.EndsWith(".zip"))
        {
            destinationZipFilePath += ".zip";
        }

        if (!Directory.Exists(sourceFolderPath))
        {
            throw new DirectoryNotFoundException("The folder to create a zip file doesn't exist");
        }

        // Tao file zip: https://stackoverflow.com/a/22444096/7182661

        FastZip fastZip = new FastZip();

        bool recurse = true; // Include all files by recursing through the directory structure
        string filter = null; // Dont filter any files at all
        fastZip.CreateZip(destinationZipFilePath, sourceFolderPath, recurse, filter);
    }

    public void ExtractZipToFolder(string sourceZipFilePath, string destinationFolder)
    {
        if (!File.Exists(sourceZipFilePath))
        {
            throw new FileNotFoundException("The zip file needed to extract doesn't exist");
        }

        FastZip fastZip = new FastZip();
        string fileFilter = null;

        // Will always overwrite if target filenames already exist
        fastZip.ExtractZip(sourceZipFilePath, destinationFolder, fileFilter);
    }
}