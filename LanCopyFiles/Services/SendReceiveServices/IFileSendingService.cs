using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanCopyFiles.Models;

namespace LanCopyFiles.Services.SendReceiveServices;

public interface IFileSendingService
{
    // Event cap nhat tien trinh
    public event EventHandler<FilesSendingProgressInfoArgs> FilesSendingProgressChanged;
    public Task<List<bool>> SendFilesToDestinationPC();
    public FilesSendingProgressInfo ReportProgress();
}