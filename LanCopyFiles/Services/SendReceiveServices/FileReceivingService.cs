using System;
using LanCopyFiles.Services.StorageServices.FilePrepare;
using LanCopyFiles.TransferFilesEngine.Server;
using log4net;

namespace LanCopyFiles.Services.SendReceiveServices;

public class FileReceivingService: IDisposable
{
    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    // private int OPEN_PORT = 8085;

    //Nguon: https://stackoverflow.com/a/71522082/7182661
    // private EftServer _server = new EftServer(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\", 8085);

    // private readonly BackgroundWorker _worker;

    private TFEServer _server =
        new TFEServer(TempFolderNames.ReceiveTempFolderPath + "\\", 8085);
    // private readonly Thread _receiverThread;

    private static FileReceivingService _instance;

    public static FileReceivingService Instance
    {
        get { return _instance ??= new FileReceivingService(); }

    }

    private FileReceivingService()
    {
        // // Nguon: https://stackoverflow.com/a/634145/7182661
        //
        //
        // // Khoi chay server: https://stackoverflow.com/questions/6481304/how-to-use-a-backgroundworker
        //
        // _worker = new BackgroundWorker();
        // _worker.WorkerSupportsCancellation = true;
        //
        // _worker.DoWork += (sender, args) =>
        // {
        //     try
        //     {
        //         //Check if there is a request to cancel the process
        //         if (_worker.CancellationPending)
        //         {
        //             args.Cancel = true;
        //             return;
        //         }
        //
        //         _server.StartServer().GetAwaiter().GetResult();
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show("An error has happened: " + ex.ToString());
        //     }
        // };

        
    }

    public async void StartService()
    {
        Log.Info("Server nhan file bat dau chay");

        await _server.StartServer();

    }

    public void StopService()
    {
        // //Check if background worker is doing anything and send a cancellation if it is
        // if (_worker.IsBusy)
        // {
        //     _worker.CancelAsync();
        // }
        // // Trace.WriteLine("Stop thread");
        Log.Info("Server nhan file da dung lai");
    }

    public void Dispose()
    {
        StopService();
    }


    public event EventHandler<TFEServerReceivingArgs> DataStartReceivingOnServer
    {
        add => _server.StartReceivingEvent += value;
        remove => _server.StartReceivingEvent -= value;
    }

    public event EventHandler<TFEServerReceivingArgs> DataFinishReceivingOnServer
    {
        add => _server.FinishReceivingEvent += value;
        remove => _server.FinishReceivingEvent -= value;
    }
}