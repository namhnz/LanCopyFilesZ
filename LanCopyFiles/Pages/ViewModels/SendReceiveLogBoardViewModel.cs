using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LanCopyFiles.Events;
using LanCopyFiles.Models;
using LanCopyFiles.Services.SendReceiveLogServices;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace LanCopyFiles.Pages.ViewModels;

public class SendReceiveLogBoardViewModel : BindableBase
{
    private ISendReceiveLogDataService _logDataService;
    private readonly IEventAggregator _eventAggregator;

    public ObservableCollection<SendReceiveLogItem> AllLogs { get; set; }

    public SendReceiveLogBoardViewModel(ISendReceiveLogDataService logDataService, IEventAggregator eventAggregator)
    {
        _logDataService = logDataService;
        _eventAggregator = eventAggregator;

        InitCommands();

        AllLogs = new ObservableCollection<SendReceiveLogItem>();

        LoadWithNewLogs();

        _eventAggregator.GetEvent<SendReceiveLogAddEvent>().Subscribe(AddNewLogs);
    }

    public ICommand ClearAllLogsCommand { get; private set; }

    private void InitCommands()
    {
        ClearAllLogsCommand = new DelegateCommand(ClearAllLogs);
    }

    private void AddNewLogs(List<SendReceiveLogItem> newLogs)
    {
        _logDataService.AddNewLogs(newLogs);
        foreach (var newLog in newLogs)
        {
            AllLogs.Insert(0, newLog);
        }
    }

    private void ClearAllLogs()
    {
        _logDataService.ClearAllLogs();
        AllLogs.Clear();
    }

    private int _perPage = 20;
    private int _currentPage = -1;

    public void LoadWithNewLogs()
    {
        var maxPage = (int)Math.Ceiling((double)_logDataService.AllLogs.Count / _perPage);

        if (_currentPage < maxPage - 1)
        {
            _currentPage++;

            var currentDisplayLogs = AllLogs.ToList();
            var newLoadLogs = _logDataService.AllLogs.Take((_currentPage + 1) * _perPage)
                .Except(currentDisplayLogs);
            AllLogs.AddRange(newLoadLogs.Reverse());
        }
    }
}