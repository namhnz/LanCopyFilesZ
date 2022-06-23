using System;

namespace LanCopyFiles.Models;

public class FilesSendingProgressInfoArgs: EventArgs
{
    public string SendingFileName { get; set; }
    public int TotalSendFilesCount { get; set; }
    public double TotalSendingPercentage { get; set; }

    public FilesSendingProgressInfoArgs(FilesSendingProgressInfo progressInfo)
    {
        SendingFileName = progressInfo.SendingFileName;
        TotalSendFilesCount = progressInfo.TotalSendFilesCount;
        TotalSendingPercentage = progressInfo.TotalSendingPercentage;
    }

    public FilesSendingProgressInfoArgs()
    {
        
    }
}