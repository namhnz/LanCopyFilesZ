using System;
using System.IO;

namespace LanCopyFiles.Services.FilePrepare;

public class TempFolderNames
{
    public static readonly string SendTempFolder = "send-temp";
    public static readonly string ReceiveTempFolder = "receive-temp";

    public static string SendTempFolderPath
    {
        get
        {
            var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;

            var sendTempFolderPath = Path.Combine(currentAppFolder, SendTempFolder);
            return sendTempFolderPath;
        }
    }

    public static string ReceiveTempFolderPath
    {
        get
        {
            var currentAppFolder = AppDomain.CurrentDomain.BaseDirectory;

            var receiveTempFolderPath = Path.Combine(currentAppFolder, ReceiveTempFolder);
            return receiveTempFolderPath;
        }
    }
}