using System;
using System.Diagnostics;
using LanCopyFiles.Events;
using LanCopyFiles.Services.IPAddressManager;
using LanCopyFiles.Services.StorageServices.FilePrepare;
using LanCopyFiles.TransferFilesEngine.Server;
using log4net;
using Prism.Events;

namespace LanCopyFiles.Services.SendReceiveServices;

public class FileReceivingService : IDisposable, IFileReceivingService
{
    private readonly IEventAggregator _eventAggregator;

    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    
    private TFEServer _server =
        new TFEServer(TempFolderNames.ReceiveTempFolderPath + "\\", IPAddressValidator.APP_DEFAULT_PORT);

    public FileReceivingService(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;

        
        
    }

    public async void StartService()
    {
        Log.Info("Server nhan file bat dau chay");

        try
        {
            await _server.StartServer();

            _server.StartReceivingEvent += (sender, args) =>
            {
                _eventAggregator.GetEvent<DataStartReceivingOnServerEvent>().Publish(args);
            };

            _server.FinishReceivingEvent += (sender, args) =>
            {
                _eventAggregator.GetEvent<DataFinishReceivingOnServerEvent>().Publish(args);
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            Debug.WriteLine(ex);
        }
    }

    public void StopService()
    {
        Log.Info("Server nhan file da dung lai");
    }

    public void Dispose()
    {
        StopService();
    }


    // public event EventHandler<TFEServerReceivingArgs> DataStartReceivingOnServer
    // {
    //     add => _server.StartReceivingEvent += value;
    //     remove => _server.StartReceivingEvent -= value;
    // }
    //
    // public event EventHandler<TFEServerReceivingArgs> DataFinishReceivingOnServer
    // {
    //     add => _server.FinishReceivingEvent += value;
    //     remove => _server.FinishReceivingEvent -= value;
    // }
}