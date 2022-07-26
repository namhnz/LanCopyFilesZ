using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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

        private static Mutex _mutex = null;
        
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
            // Thoat app neu phat hien dang chay: https://stackoverflow.com/questions/14506406/wpf-single-instance-best-practices?answertab=trending#tab-top

            const string appName = "LanCopyFiles";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application  
                MessageBox.Show("An instance is already running...");
                Application.Current.Shutdown();
            }


            log4net.Config.XmlConfigurator.Configure();
            Log.Info("LanCopyFiles khoi chay");

            base.OnStartup(e);

        }


    }

    

}
