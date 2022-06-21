using System;
using System.Collections.Generic;
using System.IO;
using EasyFileTransfer;
using log4net;

namespace LanCopyFiles.Services;

public class ThingSendService
{
    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private bool SendThingToDestinationPC(string thingPath, string destinationPCIPAddress)
    {
        try
        {
            var response = EftClient.Send(thingPath, destinationPCIPAddress, IPAddressValidator.APP_DEFAULT_PORT);
            return response.status == 1;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return false;
        }
    }

    public List<bool> SendThingsToDestinationPC(string[] thingPaths, string destinationPCIPAddress)
    {
        var sendFileResults = new List<bool>();

        for (int i = 0; i < thingPaths.Length; i++)
        {
            // _fileOrFolderCopyingIndex = i;
            string thingFullPath = thingPaths[i];

            if (File.Exists(thingFullPath))
            {
                // Dung de hien thi len thong bao trang thai
                // _copyingFileOrFolderName = Path.GetFileName(thingFullPath);

                var sendThingResult = SendThingToDestinationPC(thingFullPath, destinationPCIPAddress);
                sendFileResults.Add(sendThingResult);
            }
            else
            {
                // throw new FileNotFoundException($"File/Folder {thingPaths[i]} doesn't exist");
                Log.Error($"File/Folder {thingPaths[i]} doesn't exist");
                sendFileResults.Add(false);
            }
        }

        // _updateProgressBarTimer.Stop();

        return sendFileResults;
    }
}