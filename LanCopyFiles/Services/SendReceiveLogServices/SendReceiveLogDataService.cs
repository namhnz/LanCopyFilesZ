using System.Collections.Generic;
using System.IO;
using LanCopyFiles.Models;
using System.Text.Json;
namespace LanCopyFiles.Services.SendReceiveLogServices;

public class SendReceiveLogDataService
{
    private static SendReceiveLogDataService _instance;

    public static SendReceiveLogDataService Instance => _instance ??= new SendReceiveLogDataService();

    private SendReceiveLogDataService()
    {
        AllLogs = ReadAll();
    }

    private string _dataFileName = @"Data\sendreceive-log.json";

    public List<SendReceiveLogItem> AllLogs { get; set; }

    private void WriteAll(List<SendReceiveLogItem> data)
    {
        if (!Directory.Exists(@"Data"))
        {
            Directory.CreateDirectory(@"Data");
        }

        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(_dataFileName, json);
    }

    private List<SendReceiveLogItem> ReadAll()
    {
        if (File.Exists(_dataFileName))
        {
            string jsonText = File.ReadAllText(_dataFileName);

            var data = JsonSerializer.Deserialize<List<SendReceiveLogItem>>(jsonText);

            return data;
        }
        else
        {
            return new List<SendReceiveLogItem>();
        }
    }

    public void AddNewLogs(List<SendReceiveLogItem> logItems)
    {
        foreach (var logItem in logItems)
        {
            if (logItem.Id < 0) // Id chua duoc chinh sua
            {
                logItem.Id = AllLogs.Count;
                AllLogs.Add(logItem);
            }
        }

        WriteAll(AllLogs);
    }

    public void ClearAllLogs()
    {
        AllLogs = new List<SendReceiveLogItem>();
        WriteAll(AllLogs);
    }
}