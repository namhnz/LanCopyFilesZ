using System;
using System.Windows.Threading;

namespace LanCopyFiles.Services.SendReceiveServices;

public class FileSendingProgressUpdater
{
    // Nguon: https://stackoverflow.com/a/11560151/7182661
    private DispatcherTimer _updateProgressTimer;

    public FileSendingProgressUpdater(Action onUpdate)
    {
        _updateProgressTimer = new DispatcherTimer();
        _updateProgressTimer.Tick += (sender, args) =>
        {
            onUpdate.Invoke();
        };
        _updateProgressTimer.Interval = new TimeSpan(0, 0, 0, 1);
    }
    
}