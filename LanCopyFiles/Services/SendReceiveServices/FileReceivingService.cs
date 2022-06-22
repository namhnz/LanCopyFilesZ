using System;
using System.IO;
using System.Threading;
using System.Windows;
using EasyFileTransfer;
using EasyFileTransfer.Model;
using LanCopyFiles.Services.FilePrepare;
using log4net;

namespace LanCopyFiles.Services.SendReceiveServices;

public class FileReceivingService: IDisposable
{
    private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    // private int OPEN_PORT = 8085;

    //Nguon: https://stackoverflow.com/a/71522082/7182661
    private bool _running = false;
    // private EftServer _server = new EftServer(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\", 8085);
    private EftServer _server =
        new EftServer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TempFolderNames.ReceiveTempFolder) + "\\",
            8085);
    private readonly Thread _receiverThread;

    private static FileReceivingService _instance;

    public static FileReceivingService Instance
    {
        get { return _instance ??= new FileReceivingService(); }

    }

    private FileReceivingService()
    {
        // Nguon: https://stackoverflow.com/a/634145/7182661
        

        // Khoi chay server
        _receiverThread = new Thread(() =>
        {
            try
            {
                _server.StartServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has happened: " + ex.ToString());
            }

        });
        _receiverThread.IsBackground = true;
    }

    public void StartService()
    {
        _running = true;
        _receiverThread.Start();

    }

    public void StopService()
    {
        _running = false;
        _receiverThread.Join();

        // Trace.WriteLine("Stop thread");
        Log.Info("Thread nhan file da tam dung");
    }

    public void Dispose()
    {
        StopService();
    }


    public event EventHandler<DataReceivingArgs> DataStartReceivingOnServer
    {
        add { _server.DataStartReceiving += value; }
        remove { _server.DataStartReceiving -= value; }
    }

    public event EventHandler<DataReceivingArgs> DataFinishReceivingOnServer
    {
        add { _server.DataFinishReceiving += value; }
        remove { _server.DataFinishReceiving -= value; }
    }
}