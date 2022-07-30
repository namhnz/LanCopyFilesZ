using System.Collections.Generic;
using System.IO;
using System.Linq;
using LanCopyFiles.Extensions;
using LanCopyFiles.Services.PackingServices;

namespace LanCopyFiles.Services.StorageServices.FilePrepare
{
    public class SendingTempFolder: ISendingTempFolder
    {
        private readonly IZipService _zipService;

        public SendingTempFolder(IZipService zipService)
        {
            _zipService = zipService;
        }

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
                _zipService.CompressFolderToZip(sourceFolderPath, destinationZipFilePath);
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
    }
}