using System;
using System.Collections.Generic;
using System.IO;
using EasyFileTransfer;
using LanCopyFiles.Models;
using log4net;

namespace LanCopyFiles.Services.SendReceiveServices;

public class FileSendingService
{
    private readonly string _destinationPCIPAddress;
    private readonly int _totalFilesCount;

    private string _sendingFileName;
    private int _sendingFileIndex;
    // private double _totalSendingPercentage;

    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public FileSendingService(string[] filePaths, string destinationPCIPAddress)
    {
        _totalFilesCount = filePaths.Length;
        _destinationPCIPAddress = destinationPCIPAddress;
    }

    #region Chuc nang ho tro

    private bool SendFileToDestinationPC(string filePath, string destinationPCIPAddress)
    {
        try
        {
            var response = EftClient.Send(filePath, destinationPCIPAddress, IPAddressValidator.APP_DEFAULT_PORT);
            return response.status == 1;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return false;
        }
    }

    #endregion


    // Chi gui cac file, khong gui folder; neu gui cac folder thi nen lai thanh file zip de gui di
    public List<bool> SendFilesToDestinationPC(string[] filePaths)
    {
        var sendFileResults = new List<bool>();

        for (int i = 0; i < filePaths.Length; i++)
        {
            // _fileOrFolderCopyingIndex = i;
            string thingFullPath = filePaths[i];

            if (File.Exists(thingFullPath))
            {
                // Dung de hien thi len thong bao trang thai
                // _copyingFileOrFolderName = Path.GetFileName(thingFullPath);

                var sendThingResult = SendFileToDestinationPC(thingFullPath, _destinationPCIPAddress);
                sendFileResults.Add(sendThingResult);
            }
            else
            {
                // throw new FileNotFoundException($"File/Folder {filePaths[i]} doesn't exist");
                Log.Error($"File {thingFullPath} doesn't exist");
                sendFileResults.Add(false);
            }
        }

        // _updateProgressBarTimer.Stop();

        return sendFileResults;
    }

    #region Bao cao

    public FilesSendingProgressInfo ReportProgress()
    {
        var progressInfo = new FilesSendingProgressInfo
        {
            TotalSendFilesCount = _totalFilesCount,
            SendingFileName = _sendingFileName
        };

        // Tinh toan tien trinh hien tai
        // progressInfo.TotalSendingPercentage = _totalSendingPercentage;
        if (_totalFilesCount > 0)
        {
            progressInfo.TotalSendingPercentage = EftClient.ProgressValue * 1 /
                                                  (double)_totalFilesCount +
                                                  100 * _sendingFileIndex /
                                                  (double)_totalFilesCount;
        }
        else
        {
            progressInfo.TotalSendingPercentage = 0;
        }

        

        return progressInfo;
    }

    #endregion
}