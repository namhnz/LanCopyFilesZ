using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LanCopyFiles.Extensions;
using LanCopyFiles.Services.PackingServices;

namespace LanCopyFiles.Services.FilePrepare
{
    public class SendingTempFolder
    {
        public void Add(string sourceThingPath)
        {
            if (FileFolderDetector.IsPathFolder(sourceThingPath))
            {
                // Neu duong dan la folder thi nen lai vao cho vao thu muc send temp

                var sourceFolderPath = sourceThingPath;

                // Nen thu muc nguon thanh file zip vao dua vao thu muc temp

                // Lay ten thu muc nguon: https://stackoverflow.com/a/5229311/7182661
                string sourceFolderName = new DirectoryInfo(sourceFolderPath).Name;

                // Ten file zip bao gom full path
                var destinationZipFilePath =
                    Path.Combine(TempFolderNames.SendTempFolderPath, sourceFolderName + ".zip");
                // Kiem tra xem file zip da ton tai hay chua, neu co thi xoa file zip do di
                // if (File.Exists(destinationZipFilePath))
                // {
                //     File.Delete(destinationZipFilePath);
                // }

                // Tao file nen vao trong thu muc temp
                ZipService.Instance.CompressFolderToZip(sourceFolderPath, destinationZipFilePath);
            }
            else
            {
                // Neu duong dan la file thi chi can copy den thu muc send temp

                var sourceFilePath = sourceThingPath;

                // Lay ten file nguon
                var sourceFileName = Path.GetFileName(sourceFilePath);

                // Kiem tra neu la file zip thi them duoi .iszipfile vao cuoi ten file, vi du: file1.zip.iszipfile
                if (sourceFileName.EndsWith(".zip"))
                {
                    sourceFileName += ".iszipfile";
                }

                // Duong dan den file dich sau khi copy
                var destinationFilePath = Path.Combine(TempFolderNames.SendTempFolderPath, sourceFileName);

                // Copy file vao thu muc temp
                File.Copy(sourceFilePath, destinationFilePath, true);
            }
        }

        public void AddMany(string[] sourceThingPaths)
        {
            foreach (var sourceThingPath in sourceThingPaths)
            {
                Add(sourceThingPath);
            }
        }

        public void Delete(string fileName)
        {
            var filePath = Path.Combine(TempFolderNames.SendTempFolderPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public string Get(string fileName)
        {
            var filePath = Path.Combine(TempFolderNames.SendTempFolderPath, fileName);
            return File.Exists(filePath) ? filePath : null;
        }

        public IEnumerable<string> GetMany(string[] fileNames)
        {
            return fileNames.Select(Get);
        }

        public IEnumerable<string> GetAll()
        {
            // Lay toan bo file trong thu muc temp
            return Directory.GetFiles(TempFolderNames.SendTempFolderPath, "*.*", SearchOption.TopDirectoryOnly);
        }

        // public static void EnsureSendTempFolderExist()
        // {
        //     // // Lay thu muc dang chay app
        //     // var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;
        //     //
        //     // // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        //     // var copyTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.SendTempFolder);
        //     //
        //     // if (!Directory.Exists(copyTempFolderPath))
        //     // {
        //     //     Directory.CreateDirectory(copyTempFolderPath);
        //     // }
        // }

        // public static void PackFileReadyForCopying(string sourceFilePath)
        // {
        //     // if (!File.Exists(sourceFilePath))
        //     // {
        //     //     throw new FileNotFoundException("Khong tim thay file can copy den thu muc temp");
        //     // }
        //
        //     // // Lay thu muc dang chay app
        //     // var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;
        //     //
        //     // // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        //     // var copyTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.SendTempFolder);
        //     //
        //     // if (!Directory.Exists(copyTempFolderPath))
        //     // {
        //     //     Directory.CreateDirectory(copyTempFolderPath);
        //     // }
        //
        //     // // Lay ten file nguon
        //     // var sourceFileName = Path.GetFileName(sourceFilePath);
        //     //
        //     // // Duong dan den file dich sau khi copy
        //     // var destinationFilePath = Path.Combine(copyTempFolderPath, sourceFileName);
        //     //
        //     // // Copy file vao thu muc temp
        //     // File.Copy(sourceFilePath, destinationFilePath, true);
        // }

        // public static void PackFolderReadyForCopying(string sourceFolderPath)
        // {
        //     // // Lay thu muc dang chay app
        //     // var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;
        //     //
        //     // // Neu chua tao thu muc temp trong thu muc chay app thi tao thu muc temp
        //     // var copyTempFolderPath = Path.Combine(currentAppFolder, TempFolderNames.SendTempFolder);
        //     //
        //     // if (!Directory.Exists(copyTempFolderPath))
        //     // {
        //     //     Directory.CreateDirectory(copyTempFolderPath);
        //     // }
        //
        //     // Nen thu muc nguon thanh file zip vao dua vao thu muc temp
        //
        //     // Lay ten thu muc nguon: https://stackoverflow.com/a/5229311/7182661
        //     // string sourceFolderName = new DirectoryInfo(sourceFolderPath).Name;
        //     //
        //     // // Ten file zip bao gom full path
        //     // var destinationZipFilePath = Path.Combine(copyTempFolderPath, sourceFolderName + ".zip");
        //     // // Kiem tra xem file zip da ton tai hay chua, neu co thi xoa file zip do di
        //     // if (File.Exists(destinationZipFilePath))
        //     // {
        //     //     File.Delete(destinationZipFilePath);
        //     // }
        //     //
        //     // // Tao file nen vao trong thu muc temp
        //     // CompressFolderToZip(destinationZipFilePath, sourceFolderPath);
        // }

        // private static void CompressFolderToZip(string destinationZipFileName, string sourceFolderPath)
        // {
        //     // Them duoi .zip vao file neu chua co
        //     if (!destinationZipFileName.EndsWith(".zip"))
        //     {
        //         destinationZipFileName += ".zip";
        //     }
        //
        //     if (!Directory.Exists(sourceFolderPath))
        //     {
        //         throw new DirectoryNotFoundException("Khong tim thay thu muc can tao file zip");
        //     }
        //
        //     // Tao file zip: https://stackoverflow.com/a/22444096/7182661
        //
        //     FastZip fastZip = new FastZip();
        //
        //     bool recurse = true;  // Include all files by recursing through the directory structure
        //     string filter = null; // Dont filter any files at all
        //     fastZip.CreateZip(destinationZipFileName, sourceFolderPath, recurse, filter);
    }
}