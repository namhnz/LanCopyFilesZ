using System.Collections.Generic;
using LanCopyFiles.Events;
using LanCopyFiles.Models;
using Prism.Events;
using Prism.Mvvm;

namespace LanCopyFiles.Pages.ViewModels;

public class ReceiveFileBoardViewModel: BindableBase
{
    private readonly IEventAggregator _eventAggregator;

    public ReceiveFileBoardViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    public void AddNewReceiveLog(SendReceiveLogItem logItem)
    {
        _eventAggregator.GetEvent<SendReceiveLogAddEvent>().Publish(new List<SendReceiveLogItem>()
        {
            logItem
        });
    }
}