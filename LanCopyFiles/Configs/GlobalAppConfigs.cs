using System;
using System.IO;
using SharpConfig;

namespace LanCopyFiles.Configs;

public class GlobalAppConfigs: IGlobalAppConfigs
{
    private const string GLOBAL_APP_CONFIGS_FILE_NAME = "user_data.cfg";

    #region Luu lai cac cai dat

    public SendFilesConfigs SendFilesConfigs { get; private set; }
    private Configuration _config;

    public GlobalAppConfigs()
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
        
    }

    private void SaveConfigs()
    {
        _config.SaveToFile(GLOBAL_APP_CONFIGS_FILE_NAME);
    }

    #endregion
}