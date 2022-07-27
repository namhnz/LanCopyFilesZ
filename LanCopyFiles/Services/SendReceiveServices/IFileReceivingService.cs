using System;
using LanCopyFiles.TransferFilesEngine.Server;

namespace LanCopyFiles.Services.SendReceiveServices;

public interface IFileReceivingService
{
    public void StartService();
    public void StopService();
    public void Dispose()
    {
        StopService();
    }


    public event EventHandler<TFEServerReceivingArgs> DataStartReceivingOnServer;

    public event EventHandler<TFEServerReceivingArgs> DataFinishReceivingOnServer;
}