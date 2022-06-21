using System;
using System.IO;
using SharpConfig;

namespace LanCopyFiles.Configs;

public class GlobalAppConfigs
{
    // Su dung singleton
    private static GlobalAppConfigs _instance;

    public static GlobalAppConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GlobalAppConfigs();
            }
            return _instance;
        }
        set { _instance = value; }
    }


    private const string GLOBAL_APP_CONFIGS_FILE_NAME = "user_data.cfg";

    #region Luu lai cac cai dat

    public SendFilesConfigs SendFilesConfigs { get; private set; }
    private Configuration _config;

    private GlobalAppConfigs()
    {
        LoadConfigs();
    }

    private void LoadConfigs()
    {
        if (File.Exists(GLOBAL_APP_CONFIGS_FILE_NAME))
        {
            _config = Configuration.LoadFromFile(GLOBAL_APP_CONFIGS_FILE_NAME);
        }
        else
        {
            _config = new Configuration();
        }

        SendFilesConfigs = new SendFilesConfigs(_config["SendFilesConfigs"], SaveConfigs);

        // Lay cac gia tri cac duong dan da luu
        // var ipCopyToAddress = _destinationPCIPAddressConfigSection["IPAddress"].StringValue;
        // if (!string.IsNullOrEmpty(ipCopyToAddress))
        // {
        //     destinationPCIPAddressTextBox.Text = ipCopyToAddress;
        // }

        // var ipCopyToPort = _destinationPCIPAddressConfigSection["Port"].IntValue;
        // if (ipCopyToPort > 0 && ipCopyToPort <= 65535)
        // {
        //     serverPortTextBox.Text = ipCopyToPort.ToString();
        // }
    }

    private void SaveConfigs()
    {
        _config.SaveToFile(GLOBAL_APP_CONFIGS_FILE_NAME);
    }

    #endregion
}