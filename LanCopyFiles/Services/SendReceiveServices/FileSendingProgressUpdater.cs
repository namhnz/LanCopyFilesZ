using System;
using System.Windows;
using System.Windows.Threading;

namespace LanCopyFiles.Services.SendReceiveServices;

public class FileSendingProgressUpdater
{
    // Nguon: https://stackoverflow.com/a/11560151/7182661
    private DispatcherTimer _updateProgressTimer;
    private Action _onUpdate;

    public FileSendingProgressUpdater(Action onUpdate)
    {
        _onUpdate = onUpdate;

        _updateProgressTimer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher);
        _updateProgressTimer.Tick += UpdateProgressTimerOnTick;
        _updateProgressTimer.Interval = new TimeSpan(0, 0, 0, 1);
    }

    private void UpdateProgressTimerOnTick(object? sender, EventArgs e)
    {
        _onUpdate.Invoke();
    }

    public void StartUpdater()
    {
        _updateProgressTimer.Start();
    }

    public void StopUpdater()
    {
        // Cap nhat lan cuoi truoc khi tat timer
        _onUpdate.Invoke();

        // Tat timer
        _updateProgressTimer.Stop();
    }
}