using System;

namespace EasyFileTransfer.Model;

public class DataReceivingArgs : EventArgs
{
    public string ReceivingFileName { get; set; }
}