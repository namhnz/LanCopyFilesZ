namespace LanCopyFiles.Models;

public class FilesSendingProgressInfo
{
    public string SendingFileName { get; set; }
    public int TotalSendFilesCount { get; set; }
    public double TotalSendingPercentage { get; set; }
}