using System;
using SharpConfig;

namespace LanCopyFiles.Configs;

public class SendFilesConfigs
{
    private readonly Section _sendFilesConfigSection;
    private Action _executeWhenConfigChanged;
    public SendFilesConfigs(Section sendFilesConfigSection, Action executeWhenConfigChanged)
    {
        _sendFilesConfigSection = sendFilesConfigSection;
        _executeWhenConfigChanged = executeWhenConfigChanged;
    }

    private string GetConfigValue(string savedKey)
    {
        return _sendFilesConfigSection[savedKey].StringValue;
    }

    public string GetSavedDestinationPCIPAddressConfigValue()
    {
        return GetConfigValue("LastSendDestinationPCIPAddress");
    }

    public void SetSavedDestinationPCIPAddressConfigValue(string ipAddress)
    {
        _sendFilesConfigSection["LastSendDestinationPCIPAddress"].StringValue = ipAddress;
        _executeWhenConfigChanged.Invoke();
    }
}