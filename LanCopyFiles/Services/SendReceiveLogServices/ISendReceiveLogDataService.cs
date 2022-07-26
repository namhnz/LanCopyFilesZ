using System.Collections.Generic;
using LanCopyFiles.Models;

namespace LanCopyFiles.Services.SendReceiveLogServices;

public interface ISendReceiveLogDataService
{
    public List<SendReceiveLogItem> AllLogs { get; set; }

    public void AddNewLogs(List<SendReceiveLogItem> logItems);

    public void ClearAllLogs();
}