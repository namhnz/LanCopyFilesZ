using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LanCopyFiles.Models;
using LanCopyFiles.Services.SendReceiveLogServices;
using Prism.Commands;
using Prism.Mvvm;

namespace LanCopyFiles.Pages.ViewModels;

public class SendReceiveLogBoardViewModel : BindableBase
{
    public ObservableCollection<SendReceiveLogItem> AllLogs { get; set; }

    public SendReceiveLogBoardViewModel()
    {
        InitCommands();

        // AllLogs = new ObservableCollection<SendReceiveLogItem>()
        // {
        //     new SendReceiveLogItem()
        //     {
        //         ThingName = "123.jpg",
        //         Direction = SendReceiveDirection.Receive,
        //         LogDate = new DateTime(2022, 7, 24),
        //         WithIPAddress = "192.168.0.1"
        //     },
        //     new SendReceiveLogItem()
        //     {
        //         ThingName = "New Folder",
        //         Direction = SendReceiveDirection.Send,
        //         LogDate = new DateTime(2022, 7, 25),
        //         WithIPAddress = "172.145.0.1"
        //     }
        // };

        AllLogs = new ObservableCollection<SendReceiveLogItem>();

        LoadWithNewLogs();
    }

    public ICommand ClearAllLogsCommand { get; private set; }

    private void InitCommands()
    {
        ClearAllLogsCommand = new DelegateCommand(ClearAllLogs);
    }

    private void ClearAllLogs()
    {
        var newLog = new SendReceiveLogItem()
        {
            ThingName = "New Folder",
            Direction = SendReceiveDirection.Send,
            LogDate = new DateTime(2022, 7, 25),
            WithIPAddress = "172.145.0.1"
        };
        SendReceiveLogDataService.Instance.AddNewLogs(new List<SendReceiveLogItem>() { newLog});
        AllLogs.Add(newLog);
    }

    private int _perPage = 20;
    private int _currentPage = -1;

    public void LoadWithNewLogs()
    {
        var maxPage = (int)Math.Ceiling((double)SendReceiveLogDataService.Instance.AllLogs.Count / _perPage);

        if (_currentPage < maxPage - 1)
        {
            _currentPage++;

            var currentDisplayLogs = AllLogs.ToList();
            var newLoadLogs = SendReceiveLogDataService.Instance.AllLogs.Take((_currentPage + 1) * _perPage)
                .Except(currentDisplayLogs);
            AllLogs.AddRange(newLoadLogs.Reverse());

            
        }
    }
    
}