using System;
using Prism.Mvvm;

namespace LanCopyFiles.Models;

public class SendReceiveLogItem: BindableBase
{
    public int Id { get; set; }
    public string ThingName { get; set; }

    public SendReceiveDirection Direction { get; set; }

    public string WithIPAddress { get; set; }

    public DateTime LogDate { get; set; }

    public SendReceiveLogItem()
    {
        Id = -1;
        Direction = SendReceiveDirection.Send;
        LogDate = DateTime.Now;
    }
}