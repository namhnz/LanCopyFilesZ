using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;

namespace LanCopyFiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        }

        private void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            //Thay doi Logging level de log tat ca moi thu: https://stackoverflow.com/questions/8926409/log4net-hierarchy-and-logging-levels
            Log.Error($"Da co loi xay ra: {e.ExceptionObject}");
            MessageBox.Show("An unexpected error has happened");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            Log.Info("LanCopyFiles khoi chay");
            
        }
    }

    

}
