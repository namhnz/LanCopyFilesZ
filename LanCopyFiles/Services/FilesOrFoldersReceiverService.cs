using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using EasyFileTransfer;

namespace LanCopyFiles.Services;

public class FilesOrFoldersReceiverService: IDisposable
{
    // private int OPEN_PORT = 8085;

    //Nguon: https://stackoverflow.com/a/71522082/7182661
    private bool _running = false;
    private EftServer _server = new EftServer(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\", 8085);
    private readonly Thread _receiverThread;

    public FilesOrFoldersReceiverService()
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

        Trace.WriteLine("Stop thread");
    }

    public void Dispose()
    {
        StopService();
    }


    public event EventHandler DataStartReceivingOnServer
    {
        add { _server.DataStartReceiving += value; }
        remove { _server.DataStartReceiving -= value; }
    }
}